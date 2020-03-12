package com.gov.rsi.dmft;

import org.junit.runner.*;

import java.util.List;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Order;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestMethodOrder;
import org.junit.jupiter.api.MethodOrderer.OrderAnnotation;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.data.mongo.DataMongoTest;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.test.context.junit4.SpringRunner;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.FilterType;

import com.gov.rsi.dmft.controllers.DmerController;
import com.gov.rsi.dmft.models.Dmer;
import com.gov.rsi.dmft.repositories.DmerRepository;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * Tests of the endpoint methods of the DMER controller. 
 * 
 * These are intended to be run against the embedded MongoDB implemented
 * in the de.flapdoodle.embed.mongo packages. Contrary to expectation,
 * the database is persisted in the <user>\AppData\Local. 
 */
@SpringBootTest
@TestMethodOrder(OrderAnnotation.class)
@Disabled
public class DmerTests {
	
	// Valid message for driver 77788899
	private static final String VALID = "{\"resourceType\":\"Bundle\",\"id\":\"74998bce-e200-43d7-8b7b-80338b17e3f5\",\"type\":\"message\",\"timestamp\":\"2020-02-05T19:37:01.5843379+00:00\",\"entry\":[{\"resource\":{\"resourceType\":\"Patient\",\"id\":\"patient1\",\"identifier\":[{\"system\":\"https://fhir.infoway-inforoute.ca/registry/NamingSystem/bc-patient-healthcare-identifier\"}],\"active\":true,\"name\":[{\"use\":\"official\"}],\"telecom\":[{\"system\":\"phone\",\"use\":\"mobile\"}],\"gender\":\"other\",\"address\":[{\"use\":\"home\",\"line\":[\"4444 Werner St.\"],\"country\":\"CA\"}]}},{\"resource\":{\"resourceType\":\"QuestionnaireResponse\",\"item\":[{\"linkId\":\"columns7PatientInformation\"},{\"linkId\":\"columns7Provider\",\"item\":[{\"linkId\":\"practitioner-nameGiven\",\"answer\":[{\"valueString\":\"Ai\"}]},{\"linkId\":\"practitioner-nameFamily\",\"answer\":[{\"valueString\":\"Bolit\"}]},{\"linkId\":\"practitioner-identifier\",\"answer\":[{\"valueDecimal\":77788812}]},{\"linkId\":\"practitioner-contactTelecom\",\"answer\":[{\"valueString\":\"(604) 777-8889\"}]},{\"linkId\":\"providerAddress\",\"item\":[{\"linkId\":\"addressType2\",\"answer\":[{\"valueString\":\"http://hl7.org/fhir/ValueSet/address-use\"}]},{\"linkId\":\"streetAddress3\",\"answer\":[{\"valueString\":\"Suite 200\"}]},{\"linkId\":\"streetAddress4\",\"answer\":[{\"valueString\":\"1212 W. Broadway St.\"}]},{\"linkId\":\"city2\",\"answer\":[{\"valueString\":\"Vancouver\"}]},{\"linkId\":\"province2\",\"answer\":[{\"valueString\":\"BC\"}]},{\"linkId\":\"postalCode2\",\"answer\":[{\"valueString\":\"V2A1L1\"}]}]}]},{\"linkId\":\"relationshipWithPatient\",\"item\":[{\"linkId\":\"relationshipwithPatient\",\"item\":[{\"linkId\":\"familyPhysicianOrNp\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"locum\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"walkIn\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"firstVisit\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"np\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"specialist\",\"answer\":[{\"valueBoolean\":false}]}]}]},{\"linkId\":\"patient\",\"item\":[{\"linkId\":\"patient-identifier\",\"answer\":[{\"valueString\":\"98961112221\"}]},{\"linkId\":\"patient-nameFamily\",\"answer\":[{\"valueString\":\"Zomarev\"}]},{\"linkId\":\"patient-nameGiven\",\"answer\":[{\"valueString\":\"Pavel\"}]},{\"linkId\":\"patient-nameGiven-middle\",\"answer\":[{\"valueString\":\"V.\"}]},{\"linkId\":\"patient-birthDate\",\"answer\":[{\"valueDateTime\":\"1990-12-29\"}]},{\"linkId\":\"patient-Gender\",\"answer\":[{\"valueString\":\"other\"}]},{\"linkId\":\"patient-contactTelecom\",\"answer\":[{\"valueString\":\"(778) 111-2220\"}]},{\"linkId\":\"patientAddress\",\"item\":[{\"linkId\":\"address.use\",\"answer\":[{\"valueString\":\"home\"}]},{\"linkId\":\"streetAddress1\",\"answer\":[{\"valueString\":\"Apt 111.\"}]},{\"linkId\":\"address.line2\",\"answer\":[{\"valueString\":\"4444 Werner St.\"}]},{\"linkId\":\"city\",\"answer\":[{\"valueString\":\"Revelstoke\"}]},{\"linkId\":\"province\",\"answer\":[{\"valueString\":\"BC\"}]},{\"linkId\":\"postalCode\",\"answer\":[{\"valueString\":\"V6A1W1\"}]}]}]},{\"linkId\":\"panel\",\"item\":[{\"linkId\":\"ReasonforExamination\",\"answer\":[{\"valueString\":\"Lyft license\"}]},\r\n" + 
			"\r\n" + 
			"{\"linkId\":\"patientDriversLicenseNumber\",\"answer\":[{\"valueDecimal\":77788899}]},\r\n" + 
			"\r\n" + 
			"{\"linkId\":\"columns7MedicalIssued\",\"answer\":[{\"valueDateTime\":\"2020-01-06\"}]},{\"linkId\":\"columns7Class\",\"answer\":[{\"valueDecimal\":5}]}]},{\"linkId\":\"panel2\",\"item\":[{\"linkId\":\"vision\",\"item\":[{\"linkId\":\"acuityLoss\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"fieldDefect\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"eyeDisease\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"other\",\"answer\":[{\"valueBoolean\":true}]}]},{\"linkId\":\"visionOther\",\"answer\":[{\"valueString\":\"visvis\"}]}]},{\"linkId\":\"panel3\",\"item\":[{\"linkId\":\"hearing\",\"item\":[{\"linkId\":\"hearingLoss\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"vertigo\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"other\",\"answer\":[{\"valueBoolean\":true}]}]},{\"linkId\":\"vertigoDetails\",\"answer\":[{\"valueString\":\"withoutWarnings\"}]},{\"linkId\":\"dateoflastvertigoepisode\",\"answer\":[{\"valueDateTime\":\"2020-01-07\"}]},{\"linkId\":\"other\",\"answer\":[{\"valueString\":\"fdffdfdfdfd\"}]}]},{\"linkId\":\"panel4\",\"item\":[{\"linkId\":\"syncope\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"cad\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"nyhaFunctionalClass\",\"answer\":[{\"valueString\":\"aaa\"}]},{\"linkId\":\"arrhythmia\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"pacemaker\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"icd\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"congestiveheartfailure\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"aneurysm\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"aneurysmSite\",\"answer\":[{\"valueString\":\"no ane\"}]},{\"linkId\":\"aneurysmSize\",\"answer\":[{\"valueString\":\"xoxo\"}]},{\"linkId\":\"peripheralvasculardisease\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"cvOther\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"cvotherdetails\",\"answer\":[{\"valueString\":\"no dets\"}]}]},{\"linkId\":\"HistoryPanel4\",\"item\":[{\"linkId\":\"columnsCvatia\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsCvatiaDate\",\"answer\":[{\"valueDateTime\":\"2020-01-06\"}]},{\"linkId\":\"columnsSeizureDisorder\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsSeizuredisorder\",\"answer\":[{\"valueString\":\"provoked\"}]},{\"linkId\":\"columnsDateofLastSeizure\",\"answer\":[{\"valueDateTime\":\"2019-12-29\"}]},{\"linkId\":\"columnsNarcolepsy\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsCongenitalConditionCerebralPalsyetc\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsProgressiveDeficitParkinsonsMsalSetc\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsStabledeficitParaplegiaNerveDamageetc\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsCognitiveImpairment\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsDementiaDiagnosis\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsGdsGlobalDementiaScale\",\"answer\":[{\"valueDecimal\":2232323233232323}]},{\"linkId\":\"columnsSignificantHeadInjury\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsOther6\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsOtherpleasespecify\",\"answer\":[{\"valueString\":\"no cns\"}]}]},{\"linkId\":\"HistoryPanel6\",\"item\":[{\"linkId\":\"columnsOxygenrequiredwhendriving\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsObstructivesleepapnea\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsCpapCompliant\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsCpapCompliantdetails\",\"answer\":[{\"valueString\":\"no cpap\"}]},{\"linkId\":\"columnsApneaHyponeaIndexAhi\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsEpworthScore\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsEpworthScore2\",\"answer\":[{\"valueDecimal\":11121}]}]},{\"linkId\":\"HistoryPanel8\",\"item\":[{\"linkId\":\"historyPanel8Diabetes\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"historyPanel8SevereHypoglycemia\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel8HypoglycemiaUnawareness\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel8StableBgControl\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel8OtherEndocrine\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"columnsOther7\",\"answer\":[{\"valueString\":\"no endo\"}]}]},{\"linkId\":\"columnsHx1Panel\",\"item\":[{\"linkId\":\"amputation\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"limb\",\"answer\":[{\"valueString\":\"sssdfsdfdsfd\"}]},{\"linkId\":\"date\",\"answer\":[{\"valueDateTime\":\"2020-01-06\"}]},{\"linkId\":\"weakness\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"rangeofmotionloss\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"musculoskeletalOther\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"other2\",\"answer\":[{\"valueString\":\"o mos\"}]}]},{\"linkId\":\"HistoryPanel7\",\"item\":[{\"linkId\":\"historyPanel7Psychosis\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel7SevereDepression\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"historyPanel7Impairedjudgmentinsight\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel7MedicationnonCompliance\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel7Stablepsychiatriccondition\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"historyPanel7OtherPsych\",\"answer\":[{\"valueBoolean\":false}]}]},{\"linkId\":\"HistoryPanel9\",\"item\":[{\"linkId\":\"columnsAlcoholordrugabuseinpast2Years\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsAlcoholrelatedseizure\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"columnsPrescribeddrugsthatcouldimpair\",\"answer\":[{\"valueBoolean\":false}]}]},{\"linkId\":\"historyOtherConditions\",\"item\":[{\"linkId\":\"columnsOtherConditions\",\"answer\":[{\"valueString\":\"otherSeeGuide\"}]},{\"linkId\":\"historyOtherConditionsOther\",\"answer\":[{\"valueString\":\"no notes\"}]}]},{\"linkId\":\"visionScreeningVisualAcuityUncorrected2\",\"item\":[{\"linkId\":\"visionScreeningVisualAcuityUncorrected\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"visionScreeningUncorrectedRight\",\"answer\":[{\"valueString\":\"1\"}]},{\"linkId\":\"visionScreeningUncorrectedLeft\",\"answer\":[{\"valueString\":\"2\"}]},{\"linkId\":\"visionScreeningUncorrectedBoth\",\"answer\":[{\"valueString\":\"3\"}]}]},{\"linkId\":\"visionScreeningVisualAcuityCorrected2\",\"item\":[{\"linkId\":\"visionScreeningVisualAcuityCorrected\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"visionScreeningCorrectedRight\",\"answer\":[{\"valueString\":\"4\"}]},{\"linkId\":\"visionScreeningCorrectedLeft\",\"answer\":[{\"valueString\":\"5\"}]},{\"linkId\":\"visionScreeningCorrectedBoth\",\"answer\":[{\"valueString\":\"6\"}]}]},{\"linkId\":\"visionScreeningVisualField\",\"item\":[{\"linkId\":\"visionScreeningVisualFieldNormal\",\"answer\":[{\"valueString\":\"abnormal\"}]},{\"linkId\":\"visionScreeningMeetCriteriaYes\",\"answer\":[{\"valueBoolean\":true}]}]},{\"linkId\":\"visionScreeningBloodPressure\",\"item\":[{\"linkId\":\"visionScreeningBloodPressureValue\",\"answer\":[{\"valueString\":\"200/100\"}]}]},{\"linkId\":\"panel5\",\"item\":[{\"linkId\":\"doespatienthaveaconditionthatmayaffectdriving\",\"answer\":[{\"valueString\":\"yes\"}]},{\"linkId\":\"recommendfollowupinYears\",\"answer\":[{\"valueDecimal\":2}]}]},{\"linkId\":\"panel6\",\"item\":[{\"linkId\":\"columns4Panel2Narrative\",\"answer\":[{\"valueString\":\"Never to drive\"}]}]},{\"linkId\":\"columns5Panel\",\"item\":[{\"linkId\":\"specialistConsult\",\"answer\":[{\"valueBoolean\":false}]},{\"linkId\":\"roadtesttoassess\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"roadtesttoassessdetails\",\"answer\":[{\"valueString\":\"2\"}]},{\"linkId\":\"restrictions\",\"answer\":[{\"valueBoolean\":true}]},{\"linkId\":\"reasonandTypeofRestrictions\",\"answer\":[{\"valueString\":\"sasas\"}]}]},{\"linkId\":\"columns5Panel2\",\"item\":[{\"linkId\":\"columns5Panel2DateConsentandCertificationGiven\",\"answer\":[{\"valueDateTime\":\"2020-01-15\"}]}]}]}}]}";

	// Valid message for driver 1234567
	private static final String ANOTHER_VALID = VALID.replace("77788899",  "1234567");
	
	// Invalid message - invalid FHIR resourceType value
	private static final String INVALID_FHIR_SYNTAX = VALID.replace("patientDriversLicenseNumber", "patientDriversLicenseNumberERROR");
	
	// Changes the practitioner id
	private static final String VALID_WITH_CHANGE = VALID.replace("77788812", "12345678");
	
	@Autowired
	private DmerController controller;
	
	@Autowired
	private DmerRepository repository;

	private static Logger log = LoggerFactory.getLogger(DmerTests.class);

	/**
	 * Insert a DMER for each of drivers 77788899 and 1234567, confirming the correct 201 Created responses 
	 */
	@Test
	@Order(1)
	public void processValidDmers() {
		dump();
		log.info("Insert two DMERS");
		Assertions.assertEquals(HttpStatus.CREATED, controller.createDmer(VALID).getStatusCode());		
		Assertions.assertEquals(HttpStatus.CREATED, controller.createDmer(ANOTHER_VALID).getStatusCode());		
	}
	
	/**
	 * Get the next DMER in the "New" queue and confirm it is from the front of the queues and that
	 * status is switched to IN_PROCESS
	 */
	@Test
	@Order(2)
	public void getNextDmer() {
		dump();
		log.info("Get Next in Queue");
		ResponseEntity response = controller.getNextNewDmer();
		Assertions.assertEquals(HttpStatus.OK, response.getStatusCode());
		Assertions.assertNotNull(response.getBody());
		Assertions.assertTrue(repository.findByLicenseNumber("77788899").get(0).getStatus().equals(Dmer.Status.IN_PROCESS));
	}
	
	/**
	 * Update the status of the DMER
	 */
	@Test
	@Order(3)
	public void updateStatus() {
		dump();
		log.info("Upate Status");
		controller.updateDmerStatus("77788899", Dmer.Status.CLEAN_PASS);
		Assertions.assertTrue(repository.findByLicenseNumber("77788899").get(0).getStatus().equals(Dmer.Status.CLEAN_PASS));
	}	
	
	/**
	 * Attempt to insert an invalid DMER (
	 */
	@Test
	@Order(4)
	public void processInvalidDmer() {
		dump();
		log.info("Process Invalid DMER");
		Assertions.assertEquals(HttpStatus.BAD_REQUEST, controller.createDmer(INVALID_FHIR_SYNTAX).getStatusCode());		
	}
	
	// Logs the license number of every DMER in the database
	private void dump(){
		List<Dmer> all = repository.findAll();
		log.info("-----------------------\n Count: " + all.size());
		for (Dmer d: all) {
			log.info(d.getLicenseNumber());
		}
	}
	
}
