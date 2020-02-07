package com.gov.rsi.DMFT.repositories;

import java.util.List;

import org.springframework.data.mongodb.repository.MongoRepository;
import com.gov.rsi.DMFT.models.Report;

/**
 * Repository for DMER reports
 */
public interface ReportRepository extends MongoRepository<Report, String> {
	
	List<Report> findByLicenseNumber(String licenseNumber);

	List<Report> findFirst1ByStatusOrderByTimeNewAsc(Report.Status status);

}
