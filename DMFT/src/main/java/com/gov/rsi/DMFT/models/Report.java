package com.gov.rsi.DMFT.models;

import java.time.LocalDateTime;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.index.Indexed;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

/**
 * A Report wraps the DMER JSON document along with status and timestamps to 
 * implement a queue
 */
@Document
@Data
public class Report {
	
	public enum Status {NEW, INPROCESS, FINISHED} 
	
	@Id
	private String			id;
	
	private Status 			status;
	private LocalDateTime	timeNew;
	private LocalDateTime	timeInProcess;
	private LocalDateTime	timeFinished;
	
	@Indexed(unique = true)
	private String			licenseNumber;	
	
	private String 			json;

	public Report(String licenseNumber, String json) {
		this.licenseNumber = licenseNumber;
		this.json = json;
	}
	
	public void setStatus(Status status) {
		this.status = status;
		switch (status) {
			case NEW: 		timeNew = LocalDateTime.now(); break;
			case INPROCESS: timeInProcess = LocalDateTime.now(); break;
			case FINISHED: 	timeFinished = LocalDateTime.now(); break;
		}
	}
}
