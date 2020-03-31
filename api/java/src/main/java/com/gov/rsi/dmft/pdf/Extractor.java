package com.gov.rsi.dmft.pdf;

import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.hl7.fhir.r4.model.BooleanType;
import org.hl7.fhir.r4.model.Bundle;
import org.hl7.fhir.r4.model.DateTimeType;
import org.hl7.fhir.r4.model.DateType;
import org.hl7.fhir.r4.model.DecimalType;
import org.hl7.fhir.r4.model.Property;
import org.hl7.fhir.r4.model.Resource;
import org.hl7.fhir.r4.model.StringType;
import org.hl7.fhir.r4.model.Type;
import org.hl7.fhir.r4.model.QuestionnaireResponse.QuestionnaireResponseItemAnswerComponent;
import org.hl7.fhir.r4.model.QuestionnaireResponse.QuestionnaireResponseItemComponent;

import com.gov.rsi.dmft.fhir.r4.Parser;

public class Extractor {
	
	private Map<String, Object> requiredItems;	
	
	public void execute(String json, Map<String, Object> requiredItems) throws IOException{
		this.requiredItems = requiredItems;
		Bundle bundle = Parser.parse(json);
		extract(bundle);
	}	
			
	private void extract(Resource resource) {
		List<Property> properties = resource.children();
		
		properties.forEach(p -> {
			if (p.hasValues()){
				if (p.isList()) {
					p.getValues().forEach(p2 -> {
						if (p2 instanceof Bundle.BundleEntryComponent) {
							extract(((Bundle.BundleEntryComponent)p2).getResource());
						}
						else {
							String className = p2.getClass().getName();
							className = className.substring(className.lastIndexOf('.') + 1);
							if (className.equals("QuestionnaireResponse$QuestionnaireResponseItemComponent")){ 
								QuestionnaireResponseItemComponent responseItem = (QuestionnaireResponseItemComponent)p2;
								List<QuestionnaireResponseItemComponent> item = responseItem.getItem();

								String linkId = responseItem.getLinkId();
								List<QuestionnaireResponseItemAnswerComponent> answers = responseItem.getAnswer();
								if (answers != null && answers.size() > 0) {
									QuestionnaireResponseItemAnswerComponent answer = answers.get(0);
									Type type = answer.getValue();
								}
								if (item!= null) {
									extractItemList(linkId, item);
								}
							}
						}
					});
				}
			}
		});	
	}
	
	
	private void extractItemList(String parentLinkId, List <QuestionnaireResponseItemComponent> itemList){
		List answerItems = new ArrayList();
		
		itemList.forEach( item -> {
			String linkId = item.getLinkId() != null ? item.getLinkId().toString() : "<No LinkID>";
			
			if (item.getItem() != null ) {
				extractItemList(linkId, item.getItem());
			}
			List<QuestionnaireResponseItemAnswerComponent> answers = item.getAnswer();
			if (answers != null) {
				answers.forEach( answer -> {
					Object value = answer.getValue();
					if (!(value instanceof String)) {
						if (value instanceof BooleanType) {
							value = ((BooleanType)value).booleanValue();
						}
						else if (value instanceof DecimalType) {
							value = Long.valueOf(((DecimalType)value).asStringValue());
						}
						else if (value instanceof DateTimeType) {
							value = ((DateTimeType)value).getValue();
						}
						else if (value instanceof DateType) {
							value = ((DateType)value).getValue();
						}
						else if (value instanceof StringType) {
							value = ((StringType)value).getValue();
						}
					}
					
					if (linkId.equals("<No LinkID>")) {
						answerItems.add(value);
					}
					else {					
						requiredItems.put(linkId, value);
					}
				});
				if (linkId.equals("<No LinkID>")) {
					requiredItems.put(parentLinkId,  answerItems);
				}
			}
		});
	}
}
