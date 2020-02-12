package com.gov.rsi.dmft.controllers;

import java.text.MessageFormat;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.gov.rsi.dmft.ApplicationProperties;
import com.gov.rsi.dmft.models.Dmer;
import com.gov.rsi.dmft.repositories.DmerRepository;
import com.jayway.jsonpath.JsonPath;
import com.jayway.jsonpath.PathNotFoundException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * Implements endpoints for DMER document requests
 */
@RestController
@RequestMapping("/queue/dmer")
public class DmerController extends AbstractController {
	
	// The JSONPath expression to find the element containing the driver's license
	private static final String LICENSE_NUMBER_PATH = ApplicationProperties.get("validation.license.number.path");
	// The linkId of the "item" element holding the license number
	private static final String LICENSE_NUMBER_ELEMENT = ApplicationProperties.get("validation.license.number.element");
	
	private static Logger log = LoggerFactory.getLogger(DmerController.class);
	
	@Autowired
	private DmerRepository repository;

	/**
	 * Fetches the oldest report with NEW status from the queue
	 * @return the DMER Json report
	 */
	@RequestMapping(value = "/", method = RequestMethod.GET)
	public ResponseEntity<?> getNextNewDmer() {
		
		List<Dmer> reports = repository.findFirst1ByStatusOrderByTimeNewAsc(Dmer.Status.NEW);

		if (reports.size() == 0) {
			// No new ones left in queue
			return responseNoContent();
		}
		
		// Switch status to In Process
		Dmer nextOne = reports.get(0);
		nextOne.setStatus(Dmer.Status.INPROCESS);
		repository.save(nextOne);
		
		return responseOkWithBody(nextOne.getJson());
	}
	
	/**
	 * Inserts a DMER JSON report into the queue
	 * @param dmerJson
	 * @return
	 */
	@RequestMapping(value = "/", method = RequestMethod.POST)
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
					return responseOkNoBody();
				}
			}
		}
		
		// Store in the queue with status NEW 
		Dmer report = new Dmer(licenseNumber, dmerJson);
		report.setStatus(Dmer.Status.NEW);
		repository.save(report);
		
		return responseCreated();	
	}
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.GET)
//	public Dmer getDmerById(@PathVariable("id") ObjectId id) {
//		
//		return repository.findBy_id(id);
//	  
//	}
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.PUT)
//	public Dmer modifyDmerById(@PathVariable("id") ObjectId id, 
//			@Valid @RequestBody Object report) {
//		
//		Dmer driversDmer = new Dmer(id, "In-Progress", report);
//		repository.save(driversDmer);
//		return driversDmer;
//	}
	
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.DELETE)
//	public void deleteDmer(@PathVariable ObjectId id) {
//		
//		repository.delete(repository.findBy_id(id));
//		
//	}	

}