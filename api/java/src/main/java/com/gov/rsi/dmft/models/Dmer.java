package com.gov.rsi.dmft.models;

import java.time.LocalDateTime;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.index.Indexed;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

/**
 * A Dmer entity wraps the DMER JSON document along with driver information, processing status, and and status change timestamps.
 * <p> 
 * The status values are representd as Strings. Each value corresponds to a 
 * sematically identical status in the DFCMS Database. Note that these
 * are a subset of the full status set.
 */
@Document
@Data
public class Dmer {
	
	/**
	 * The possible status values
	 */
	public enum Status {NEW, IN_PROCESS, CLEAN_PASS, NON_CLEAN_PASS, PRIORITY_URGENT, PRIORITY_REGULAR, PRIORITY_CASEMANAGER, ADJUDICATION, INTAKE} 
	
	@Id
	private String			id;
	
	private Status 			status;
	private LocalDateTime	timeNew;
	private LocalDateTime	timeInProcess;
	
	// timeFinished is for any status other than NEW or IN_PROCESS
	private LocalDateTime	timeFinished;
	
	@Indexed(unique = true)
	private String			licenseNumber;
	
	private String			displayName;	
	private String 			json;

	public Dmer(String licenseNumber, String json) {
		this.licenseNumber = licenseNumber;
		this.json = json;
	}
	
	public void setStatus(Status status) {
		this.status = status;
		switch (status) {
			case NEW: 			timeNew = LocalDateTime.now(); break;
			case IN_PROCESS: 	timeInProcess = LocalDateTime.now(); break;
			case CLEAN_PASS:
			case PRIORITY_URGENT:
			case PRIORITY_REGULAR:
			case PRIORITY_CASEMANAGER:
			case ADJUDICATION:
			case INTAKE:		timeFinished = LocalDateTime.now(); break;
		}
	}
}
