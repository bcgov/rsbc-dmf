package com.gov.rsi.DMFT.repositories;

import org.bson.types.ObjectId;
import org.springframework.data.mongodb.repository.MongoRepository;
import com.gov.rsi.DMFT.models.Report;


public interface ReportRepository extends MongoRepository<Report, String> {
	
	Report findBy_id(ObjectId _id);

}
