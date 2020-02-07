package com.gov.rsi.DMFT.controllers;

import java.text.MessageFormat;
import java.util.Date;
import java.util.Iterator;
import java.util.List;

import javax.validation.Valid;

//import javax.ws.rs.core.Response;

import org.bson.types.ObjectId;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.gov.rsi.DMFT.models.Report;
import com.gov.rsi.DMFT.repositories.ReportRepository;
import com.jayway.jsonpath.JsonPath;
import com.jayway.jsonpath.PathNotFoundException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * Implements endpoints for DMER document requests
 */
@RestController
@RequestMapping("/queue/dmer")
public class ReportController extends AbstractController {
	
	// The JSONPath expression to find the element containing the driver's license
	// Note that in the assumed schema, it is within an "item" element in the Questionnaire
	// resource, rather than in the "Patient" resource
	private static final String LICENSE_NUMBER_PATH = "$..item[?(@.linkId==\"{0}\")]";
	// The linkId of the "item" element holding the license number
	private static final String LICENSE_NUMBER_ELEMENT = "patientDriversLicenseNumber";
	
	private static Logger log = LoggerFactory.getLogger(ReportController.class);
	
	@Autowired
	private ReportRepository repository;

	/**
	 * Fetches the oldest report with NEW status from the queue
	 * @return the DMER Json report
	 */
	@RequestMapping(value = "/", method = RequestMethod.GET)
	public ResponseEntity<?> getNextNewReport() {
		
		List<Report> reports = repository.findFirst1ByStatusOrderByTimeNewAsc(Report.Status.NEW);

		if (reports.size() == 0) {
			// No new ones left in queue
			return responseNoContent();
		}
		
		// Switch status to In Process
		Report nextOne = reports.get(0);
		nextOne.setStatus(Report.Status.INPROCESS);
		repository.save(nextOne);
		
		return responseOkWithBody(nextOne.getJson());
	}
	
	/**
	 * Inserts a DMER JSON report into the queue
	 * @param dmerJson
	 * @return
	 */
	@RequestMapping(value = "/", method = RequestMethod.POST)
	public ResponseEntity<?> createReport(@RequestBody String dmerJson) {
		
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
		List<Report> reports = repository.findByLicenseNumber(licenseNumber);
		
		// Look for one with identical content
		if (reports != null && reports.size() > 0) {
			for(Report report: reports){
				if (report.getJson().equals(dmerJson)){
					// Ignore the duplicate
					return responseOkNoBody();
				}
			}
		}
		
		// Store in the queue with status NEW 
		Report report = new Report(licenseNumber, dmerJson);
		report.setStatus(Report.Status.NEW);
		repository.save(report);
		
		return responseCreated();	
	}
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.GET)
//	public Report getReportById(@PathVariable("id") ObjectId id) {
//		
//		return repository.findBy_id(id);
//	  
//	}
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.PUT)
//	public Report modifyReportById(@PathVariable("id") ObjectId id, 
//			@Valid @RequestBody Object report) {
//		
//		Report driversReport = new Report(id, "In-Progress", report);
//		repository.save(driversReport);
//		return driversReport;
//	}
	
	
//	@RequestMapping(value = "/{id}", method = RequestMethod.DELETE)
//	public void deleteReport(@PathVariable ObjectId id) {
//		
//		repository.delete(repository.findBy_id(id));
//		
//	}	

}