package com.gov.rsi.dmft.repositories;

import java.util.List;

import org.springframework.data.mongodb.repository.MongoRepository;
import com.gov.rsi.dmft.models.Dmer;

/**
 * Repository for {@link com.gov.rsi.dmft.models.Dmer Dmer} entities
 */
public interface DmerRepository extends MongoRepository<Dmer, String> {
	
	/**
	 * Retrieves all {@link com.gov.rsi.dmft.models.Dmer Dmer} entities for a specified licence number
	 * @param licenseNumber the license number
	 * @return a List of matching Dmers
	 */
	List<Dmer> findByLicenseNumber(String licenseNumber);

	/**
	 * For all Dmer entities with a specified status, retrieves the one with the earliest timeNew value.
	 * This thus implements a FIFO queue.
	 * @param status the desired Dmer {@link com.gov.rsi.dmft.models.Dmer.Status Status} 
	 * @return a single member List containing the earliest {@link com.gov.rsi.dmft.models.Dmer Dmer} of the specified status
	 */
	List<Dmer> findFirst1ByStatusOrderByTimeNewAsc(Dmer.Status status);

}
