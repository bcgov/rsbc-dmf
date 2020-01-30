package com.gov.rsi.DMFT.controllers;

import java.util.List;

import javax.validation.Valid;

import org.bson.types.ObjectId;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.gov.rsi.DMFT.models.Report;
import com.gov.rsi.DMFT.repositories.ReportRepository;


@RestController
@RequestMapping("/reports")
public class ReportController {
	
	@Autowired
	private ReportRepository repository;

	@RequestMapping(value = "/", method = RequestMethod.GET)
	public List<Report> getAllReports() {
		
		return repository.findAll();
	  
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.GET)
	public Report getReportById(@PathVariable("id") ObjectId id) {
		
		return repository.findBy_id(id);
	  
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.PUT)
	public Report modifyReportById(@PathVariable("id") ObjectId id, 
			@Valid @RequestBody Object report) {
		
		Report driversReport = new Report(id, "In-Progress", report);
		repository.save(driversReport);
		return driversReport;
	}
	
	@RequestMapping(value = "/", method = RequestMethod.POST)
	public Report createReport(@Valid @RequestBody Object report) {
		
		Report driversReport = new Report(ObjectId.get(), "New", report);
		repository.save(driversReport);
		return driversReport;		
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.DELETE)
	public void deleteReport(@PathVariable ObjectId id) {
		
		repository.delete(repository.findBy_id(id));
		
	}	

}