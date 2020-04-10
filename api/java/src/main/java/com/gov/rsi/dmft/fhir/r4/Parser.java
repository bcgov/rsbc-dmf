package com.gov.rsi.dmft.fhir.r4;

import ca.uhn.fhir.context.FhirContext;
import org.hl7.fhir.r4.model.Bundle;
import ca.uhn.fhir.parser.IParser;

/**
 * Wrapper for parsing the eDMER JSON into Java using the HAPI FHIR library
 * @author Owner
 *
 */
public class Parser {
	private static FhirContext context = FhirContext.forR4();

	/**
	 * Parses the JSON eDMER
	 * @param json the Json eDMER
	 * @return a HAPI FHIR Bundle object
	 */
	public static Bundle parse(String json) {
		IParser parser = context.newJsonParser();
		Bundle bundle = parser.parseResource(Bundle.class, json);
		return bundle;
	}
	
}
