package com.gov.rsi.DMFT.models;

import org.bson.types.ObjectId;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

@Document
public class Report {
	
	@Id
	private ObjectId _id;
	
	private String status;
	
	private Object report;

	public Report(ObjectId _id, String status, Object report) {
		this._id = _id;
		this.status = status;
		this.report = report;
	}

	public ObjectId get_id() {
		return _id;
	}

	public void set_id(ObjectId _id) {
		this._id = _id;
	}

	public Object getReport() {
		return report;
	}

	public void setReport(Object report) {
		this.report = report;
	}
	
	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}
	

}
