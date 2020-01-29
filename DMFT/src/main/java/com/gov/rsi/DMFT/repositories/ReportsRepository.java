package com.gov.rsi.DMFT.repositories;

import org.bson.types.ObjectId;
import org.springframework.data.mongodb.repository.MongoRepository;
import com.gov.rsi.DMFT.models.Reports;


public interface ReportsRepository extends MongoRepository<Reports, String> {
	
	Reports findBy_id(ObjectId _id);

}
