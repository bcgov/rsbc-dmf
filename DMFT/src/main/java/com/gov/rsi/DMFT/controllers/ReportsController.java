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

import com.gov.rsi.DMFT.models.Reports;
import com.gov.rsi.DMFT.repositories.ReportsRepository;


@RestController
@RequestMapping("/reports")
public class ReportsController {
	
	@Autowired
	private ReportsRepository repository;

	@RequestMapping(value = "/", method = RequestMethod.GET)
	public List<Reports> getAllReports() {
		
		return repository.findAll();
	  
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.GET)
	public Reports getReportById(@PathVariable("id") ObjectId id) {
		
		return repository.findBy_id(id);
	  
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.PUT)
	public Reports modifyReportById(@PathVariable("id") ObjectId id, 
			@Valid @RequestBody Object report) {
		
		Reports driversReport = new Reports(id, "In-Progress", report);
		repository.save(driversReport);
		return driversReport;
	}
	
	@RequestMapping(value = "/", method = RequestMethod.POST)
	public Reports createReport(@Valid @RequestBody Object report) {
		
		Reports driversReport = new Reports(ObjectId.get(), "New", report);
		repository.save(driversReport);
		return driversReport;		
	}
	
	@RequestMapping(value = "/{id}", method = RequestMethod.DELETE)
	public void deleteReport(@PathVariable ObjectId id) {
		
		repository.delete(repository.findBy_id(id));
		
	}	

}