package com.gov.rsi.dmft.controllers;

import java.io.OutputStream;
import java.text.MessageFormat;
import java.util.List;
import java.util.Optional;

import javax.servlet.http.HttpServletResponse;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.servlet.mvc.method.annotation.StreamingResponseBody;

import com.gov.rsi.dmft.ApplicationProperties;
import com.gov.rsi.dmft.models.Dmer;
import com.gov.rsi.dmft.pdf.DmerBacker;
import com.gov.rsi.dmft.repositories.DmerRepository;
import com.jayway.jsonpath.JsonPath;
import com.jayway.jsonpath.PathNotFoundException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * Implements endpoints for DMER document requests
 */
@RestController
@RequestMapping("/api/queue/dmer")
public class DmerController extends AbstractController {
	
	// The JSONPath expression to find the element containing the driver's license
	private static final String LICENSE_NUMBER_PATH = ApplicationProperties.get("validation.license.number.path");
	// The linkId of the "item" element holding the license number
	private static final String LICENSE_NUMBER_ELEMENT = ApplicationProperties.get("validation.license.number.element");
	
	// Same for family and given names
	private static final String FAMILY_NAME_PATH = ApplicationProperties.get("patient.name.family.path");
	private static final String FAMILY_NAME_ELEMENT = ApplicationProperties.get("patient.name.family.element");
	private static final String GIVEN_NAME_PATH = ApplicationProperties.get("patient.name.given.path");
	private static final String GIVEN_NAME_ELEMENT = ApplicationProperties.get("patient.name.given.element");
	
	private static Logger log = LoggerFactory.getLogger(DmerController.class);
	
	@Autowired
	private DmerRepository repository;
	
	private enum DMER_FORMAT {json, pdf}

	/**
	 * Fetches the oldest report with NEW status from the queue
	 * @return the DMER Json report
	 */
	@RequestMapping(value = "", method = RequestMethod.GET)
	public ResponseEntity<?> getNextNewDmer() {
		
		List<Dmer> reports = repository.findFirst1ByStatusOrderByTimeNewAsc(Dmer.Status.NEW);

		if (reports.size() == 0) {
			// No new ones left in queue
			log.info("New DMER queue empty");
			return responseNoContent();
		}
		
		// Switch status to In Process
		Dmer nextOne = reports.get(0);
		nextOne.setStatus(Dmer.Status.IN_PROCESS);
		repository.save(nextOne);
		
		return responseOkWithBody(nextOne.getJson());
	}
	
	/**
	 * Fetches all DMERS
	 * @return the DMER Json report
	 */
	@RequestMapping(value = "/status", method = RequestMethod.GET)
	public ResponseEntity<?> getAllDmers() {
		
		List<Dmer> reports = repository.findAll();

		if (reports.size() == 0) {
			return responseNoContent();
		}
		
		HttpHeaders headers = new HttpHeaders();
		headers.add("Access-Control-Allow-Origin", "*");
		ResponseEntity response = new ResponseEntity(reports, headers, HttpStatus.OK);
		
		return response;
	}
	
	/**
	 * Fetches a DMER identified by id as a PDF
	 * 
	 */
	@RequestMapping(value = "/{id}", method = RequestMethod.GET)
	public void getById(
			@PathVariable("id") String id, 
			@Validated @RequestParam("format") DMER_FORMAT format,
			final HttpServletResponse response) {
		
		log.info("GET pdf for dmer " + id);
		Optional<Dmer> optional = repository.findById(id);
		if (optional.isPresent()) {
			Dmer dmer = optional.get();
			if (format == DMER_FORMAT.json) {
				// TODO 
			}
			else {
				DmerBacker pdfBacker = new DmerBacker(dmer.getJson());
				try {
					OutputStream os = response.getOutputStream();
					pdfBacker.generatePdf(os);
					response.setContentType("application/pdf");
					response.setHeader("Content-Disposition", "attachment;filename=dmer.pdf");
				}
				catch (Exception e) {
					log.error("Failed to create pdf: " + e);
					response.setStatus(HttpStatus.INTERNAL_SERVER_ERROR.value());
				}
			}
		}
		else {
			response.setStatus(HttpStatus.NOT_FOUND.value());
		}
	}
	
	
	/**
	 * Deletes all DMERS
	 * @return the DMER Json report
	 */
	@RequestMapping(value = "", method = RequestMethod.DELETE)
	public ResponseEntity<?> deleteAllDmers() {		
		repository.deleteAll();
		return responseOkNoBody();
	}

	
	/**
	 * Inserts a DMER JSON report into the queue
	 * @param dmerJson the DMER document in JSON format
	 * @return a ResponseEntity
	 */
	@RequestMapping(value = "", method = RequestMethod.POST)
	public ResponseEntity<?> createDmer(@RequestBody String dmerJson) {
		
		String licenseNumber = null;
		
		// Locate the driver's license within the DMER Json text 
		try {
			String query = MessageFormat.format(LICENSE_NUMBER_PATH, LICENSE_NUMBER_ELEMENT);		
			licenseNumber = JsonPath.read(
					JsonPath.read(dmerJson, query)
					.toString(), "$[0].answer[0].valueDecimal").toString();
		}
		catch (PathNotFoundException e) {
			// Treat same as if path found but element is empty
		}
		
		// TODO: Need to apply any other criteria for a valid DL number (e.g. 7 digits only ) ?
		if (licenseNumber == null || licenseNumber.length() == 0) {
			log.warn("DMER JSON: Unable to locate DL");
			return responseBadRequest();
		}
		
		// Look for records with matching DL
		List<Dmer> reports = repository.findByLicenseNumber(licenseNumber);
		
		// Look for one with identical content
		if (reports != null && reports.size() > 0) {
			for(Dmer report: reports){
				if (report.getJson().equals(dmerJson)){
					// Ignore the duplicate
					log.info("Ignoring DMER with duplicate license number");
					return responseOkNoBody();
				}
			}
		}
		
		// Store in the queue with status NEW 
		Dmer report = new Dmer(licenseNumber, dmerJson);
		report.setStatus(Dmer.Status.NEW);

		// Extract the drivers name
		try {
			String query = MessageFormat.format(FAMILY_NAME_PATH, FAMILY_NAME_ELEMENT);		
			String familyName = JsonPath.read(
					JsonPath.read(dmerJson, query)
					.toString(), "$[0].answer[0].valueString");
			query = MessageFormat.format(GIVEN_NAME_PATH, GIVEN_NAME_ELEMENT);		
			String givenName = JsonPath.read(
					JsonPath.read(dmerJson, query)
					.toString(), "$[0].answer[0].valueString");
			report.setDisplayName(familyName + ", " + givenName);
			
		}
		catch (PathNotFoundException e) {
			// Treat same as if path found but element is empty
		}

		repository.save(report);
		
		return responseCreated();	
	}
	
	/**
	 * Updates the status of a specified DMER
	 * @param licenseNumber the license number
	 * @param status the new status
	 * @return a ResponseEntity 
	 */
	@RequestMapping(value = "", method = RequestMethod.PUT)
	public ResponseEntity updateDmerStatus(@RequestParam("id") String licenseNumber, 
			@Validated @RequestParam("status") Dmer.Status status) {
		
		// Look for records with matching DL
		List<Dmer> reports = repository.findByLicenseNumber(licenseNumber);
		if (reports == null || reports.size() == 0) {
			log.error("Cannot update DMER status: license number not found");
			return responseBadRequest("Cannot update DMER status: license number not found");
		}
		
		// Update the status and save
		Dmer report = reports.get(0);
		report.setStatus(status);
		repository.save(report);
		log.info("Received DMER for license number " + licenseNumber);
		
		return responseOkNoBody();
	}
}