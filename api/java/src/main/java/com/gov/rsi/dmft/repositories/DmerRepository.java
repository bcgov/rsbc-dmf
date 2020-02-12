package com.gov.rsi.dmft.repositories;

import java.util.List;

import org.springframework.data.mongodb.repository.MongoRepository;
import com.gov.rsi.dmft.models.Dmer;

/**
 * Repository for DMER reports
 */
public interface DmerRepository extends MongoRepository<Dmer, String> {
	
	List<Dmer> findByLicenseNumber(String licenseNumber);

	List<Dmer> findFirst1ByStatusOrderByTimeNewAsc(Dmer.Status status);

}
