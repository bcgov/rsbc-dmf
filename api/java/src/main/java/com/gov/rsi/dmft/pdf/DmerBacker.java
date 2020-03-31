package com.gov.rsi.dmft.pdf;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.apache.commons.lang.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import lombok.Data;
import net.sf.jasperreports.engine.JRException;

@Data
public class DmerBacker extends Backer{

    public Boolean         physician;                    
    public Boolean         np;                           
    public Boolean         attachedPatient;              
    public Boolean         walkInClinic;                 
    public Boolean         locum;                        
    public Boolean         specialist;                   
    public Date            columns7MedicalIssued1;       
    public String          reasonForExam2; 
    public Long            patientDL1;		
    public String          licenceClass2;
    
    public List<String>    licenceRestrictions;          
    public String          licenceRestrictionsCombined;  
    
    public Date            driverConsentDate;            
    public String          leftUncorrected;              
    public String          rightUncorrected;             
    public String          bothUncorrected;              
    public String          leftCorrected;                
    public String          rightCorrected;               
    public String          bothCorrected;              
    public String          acuityLoss;                   
    public String          acuityLossProgressiveYN;
    
    public List<String>    acuityDiagnosis;
    public String          acuityDiagnosisCombined;
    
    public String          AbnormalVFC;
    
    public List<String>    diagnosisVisualFieldImpairment;
    public String	       diagnosisVisualFieldImpairmentCombined;
    
    public Date            dateOfOnset;                  
    public String          diplopiaPresentYN;            
    public String          adequateAdjustmentYN;         
    public Boolean         prism;                        
    public Boolean         patch;                        
    public String          impairmentEvidence;           
    public String          diagnosisCausingDecline;      
    public Boolean         alzheimers;                   
    public Boolean         lewyBodyDementia;             
    public Boolean         parkinsonsWDementia;          
    public Boolean         picksDiseaseOrComplex;        
    public Boolean         vascularDementia;             
    public String          conductedScreening;           
    public Long            mocaScore;                    
    public Long            gdsScore;                     
    public Long            trailsB;                      
    public String          contributingFactors;          
    public Boolean         education;                    
    public Boolean         hearingLoss;                  
    public Boolean         infection;                    
    public Boolean         thyroidDeficiencyOrExcess;    
    public String          historyDebility;              
    public Boolean         lossStrength;                 
    public Boolean         romLoss;                      
    public Boolean         fatigue;                      
    public Boolean         lossextremityFlexion;         
    public Boolean         lossjointMobility;            
    public Boolean         losstrunkMobility;            
    public Boolean         amputation;                   
    public Boolean         rightArmAboveElbow;           
    public Boolean         rightArmBelowElbow;           
    public Boolean         leftArmAboveElbow;            
    public Boolean         leftArmBelowElbow;            
    public Boolean         rightLegBelowKnee;         
    public Boolean         leftLegBelowKnee;             
    public Boolean         rightLegAboveKnee;            
    public Boolean         leftLegAboveKnee;             
    public String          prosthesis;                   
    public String          historychronicCondition;      
    public Boolean         rheumatoidArthritis;          
    public Boolean         osteoarthritis;               
    public Boolean         degenerativeDiscDisease;      
    public Boolean         permanentSpineInjury;         
    public String          adaptiveEquipmentYN;          
    public Boolean         spinnerKnob;                  
    public Boolean         handControls;                 
    public Boolean         vehicleModifications;         
    public Boolean         yes;                          
    public Boolean         recommendFollowUp;            
    public Boolean         recommendRoadTest;            
    public Boolean         no;                           
    public Boolean         OneYear;                        
    public Boolean         TwoYears;                       
    public String          detailsCondition;             
    public Boolean         attached;                     
    public Boolean         referralCompleted;            
    public Boolean         referralRequired;             

    private Map<String, Object> requiredItems;
    private String				json;
	
	private static final String PARAM_BC_LOGO_GRAPHIC		= "BC_LOGO";
	private static final String PARAM_BOX_GRAPHIC			= "BOX";
	private static final String PARAM_BOX_CHECKED_GRAPHIC	= "BOX_CHECKED";
	
	private static final String FILE_BC_LOGO_GRAPHIC		= "BCLogo.png";
	private static final String FILE_BOX_GRAPHIC			= "Box.png";
	private static final String FILE_BOX_CHECKED_GRAPHIC	= "Box-Checked.png";
	
	private static Logger log = LoggerFactory.getLogger(DmerBacker.class);

	public static void main(String[] args) {
		
		String p1 = (new File("./")).getAbsolutePath();
		p1 = p1.substring(0, p1.lastIndexOf('.'));
		
		DmerBacker backer = new DmerBacker();
		
		String jsonPath = System.getProperty("user.dir") + "\\samples\\" + "bundle4.json";
			backer.compile("dmer");
	}
	
	public DmerBacker() {
		initialize();
	}
	
	public DmerBacker(String json) {
		initialize();
		this.json = json;
	}
	
	public void generatePdf(OutputStream os) throws IOException , JRException{
		Extractor extractor = new Extractor();
		extractor.execute(json, requiredItems);		
		populateReportFields();
		createDmerPdf(os);
	}
	
	private void createDmerPdf(DmerBacker backer, OutputStream os) throws JRException, IOException{
		
		ClassLoader loader = Thread.currentThread().getContextClassLoader();

		HashMap<String, Object> params = new HashMap<String, Object>();
		
		params.put(PARAM_BC_LOGO_GRAPHIC, 		loader.getResource(FILE_BC_LOGO_GRAPHIC).toString());
		params.put(PARAM_BOX_GRAPHIC, 			loader.getResource(FILE_BOX_GRAPHIC).toString());
		params.put(PARAM_BOX_CHECKED_GRAPHIC, 	loader.getResource(FILE_BOX_CHECKED_GRAPHIC).toString());

		ReportGenerator generator = new ReportGenerator();
		
		InputStream is = loader.getResourceAsStream("dmer.jasper");
		
		generator.generate(
				backer.asDataSource(), params, 
				is, 
				os);
	}
	
	private void createDmerPdf(OutputStream os) throws JRException, IOException{
		createDmerPdf(this, os);
	}
	
	private void initialize() {
		requiredItems = new LinkedHashMap();
		
        requiredItems.put("physician", null);                                
        requiredItems.put("np", null);                                       
        requiredItems.put("attachedPatient", null);                          
        requiredItems.put("walkInClinic", null);                             
        requiredItems.put("locum", null);                                    
        requiredItems.put("specialist", null);                               
        requiredItems.put("columns7MedicalIssued1", null);                   
        requiredItems.put("reasonForExam2", null);
        requiredItems.put("patientDL1", null);        
        requiredItems.put("licenceClass2", null);                            
        requiredItems.put("licenceRestrictions", null);                      
        requiredItems.put("driverConsentDate", null);                        
        requiredItems.put("leftUncorrected", null);                          
        requiredItems.put("rightUncorrected", null);                         
        requiredItems.put("bothUncorrected", null);                          
        requiredItems.put("leftCorrected", null);                            
        requiredItems.put("rightCorrected", null);                           
        requiredItems.put("AcuityLoss", null);                               
        requiredItems.put("AcuityLossProgressiveYN", null);                  
        requiredItems.put("AcuityDiagnosis", null);                          
        requiredItems.put("AbnormalVFC", null);                              
        requiredItems.put("DiagnosisVisualFieldImpairment", null);           
        requiredItems.put("dateOfOnset", null);                              
        requiredItems.put("diplopiaPresentYN", null);                        
        requiredItems.put("adequateAdjustmentYN", null);                     
        requiredItems.put("prism", null);                                    
        requiredItems.put("patch", null);                                    
        requiredItems.put("impairmentEvidence", null);                       
        requiredItems.put("diagnosisCausingDecline", null);                  
        requiredItems.put("alzheimers", null);                               
        requiredItems.put("lewyBodyDementia", null);                         
        requiredItems.put("parkinsonsWDementia", null);                      
        requiredItems.put("picksDiseaseOrComplex", null);                    
        requiredItems.put("vascularDementia", null);                         
        requiredItems.put("conductedScreening", null);                       
        requiredItems.put("mocaScore", null);                                
        requiredItems.put("gdsScore", null);                                 
        requiredItems.put("trailsB", null);                                  
        requiredItems.put("contributingFactors", null);                      
        requiredItems.put("education", null);                                
        requiredItems.put("hearingLoss", null);                              
        requiredItems.put("infection", null);                                
        requiredItems.put("thyroidDeficiencyOrExcess", null);                
        requiredItems.put("historyDebility", null);                          
        requiredItems.put("lossStrength", null);                             
        requiredItems.put("romLoss", null);                                  
        requiredItems.put("fatigue", null);                                  
        requiredItems.put("lossextremityFlexion", null);                     
        requiredItems.put("lossjointMobility", null);                        
        requiredItems.put("losstrunkMobility", null);                        
        requiredItems.put("amputation", null);                               
        requiredItems.put("rightArmAboveElbow", null);                       
        requiredItems.put("rightArmBelowElbow", null);                       
        requiredItems.put("leftArmAboveElbow", null);                        
        requiredItems.put("leftArmBelowElbow", null);                        
        requiredItems.put("right Leg Below Knee", null);                     
        requiredItems.put("leftLegBelowKnee", null);                         
        requiredItems.put("rightLegAboveKnee", null);                        
        requiredItems.put("leftLegAboveKnee", null);                         
        requiredItems.put("prosthesis", null);                               
        requiredItems.put("historychronicCondition", null);                  
        requiredItems.put("rheumatoidArthritis", null);                      
        requiredItems.put("osteoarthritis", null);                           
        requiredItems.put("degenerativeDiscDisease", null);                  
        requiredItems.put("permanentSpineInjury", null);                     
        requiredItems.put("adaptiveEquipmentYN", null);                      
        requiredItems.put("spinnerKnob", null);                              
        requiredItems.put("handControls", null);                             
        requiredItems.put("vehicleModifications", null);                     
        requiredItems.put("yes", null);                                      
        requiredItems.put("recommendFollowUp", null);                        
        requiredItems.put("recommendRoadTest", null);                        
        requiredItems.put("no", null);                                       
        requiredItems.put("1Year", null);                                    
        requiredItems.put("2Years", null);                                   
        requiredItems.put("detailsCondition", null);                         
        requiredItems.put("attached", null);                                 
        requiredItems.put("referralCompleted", null);                        
        requiredItems.put("referralRequired", null);                         
	}
	
	private void populateReportFields() {
        physician =                       (Boolean)     requiredItems.get("physician");                        
        np =                              (Boolean)     requiredItems.get("np");                               
        attachedPatient =                 (Boolean)     requiredItems.get("attachedPatient");                  
        walkInClinic =                    (Boolean)     requiredItems.get("walkInClinic");                     
        locum =                           (Boolean)     requiredItems.get("locum");                            
        specialist =                      (Boolean)     requiredItems.get("specialist");                       
        columns7MedicalIssued1 =          (Date)        requiredItems.get("columns7MedicalIssued1");           
        patientDL1 =                      (Long)        requiredItems.get("patientDL1");                   
        reasonForExam2 =           fromCC((String)      requiredItems.get("reasonForExam2"));                   
        licenceClass2 =            fromCC((String)      requiredItems.get("licenceClass2"));                    
        licenceRestrictions =             (List<String>)requiredItems.get("licenceRestrictions");              
        driverConsentDate =               (Date)        requiredItems.get("driverConsentDate");                
        leftUncorrected =                 (String)      requiredItems.get("leftUncorrected");                  
        rightUncorrected =                (String)      requiredItems.get("rightUncorrected");                 
        bothUncorrected =                 (String)      requiredItems.get("bothUncorrected");                  
        leftCorrected =                   (String)      requiredItems.get("leftCorrected");                    
        rightCorrected =                  (String)      requiredItems.get("rightCorrected");                   
        acuityLoss =                      (String)      requiredItems.get("AcuityLoss");                       
        acuityLossProgressiveYN =         (String)      requiredItems.get("AcuityLossProgressiveYN");          
        acuityDiagnosis =                 (List<String>)requiredItems.get("AcuityDiagnosis");                  
        AbnormalVFC =                     (String)      requiredItems.get("AbnormalVFC");                      
        diagnosisVisualFieldImpairment =  (List<String>)requiredItems.get("DiagnosisVisualFieldImpairment");   
        dateOfOnset =                     (Date)        requiredItems.get("dateOfOnset");                      
        diplopiaPresentYN =               (String)      requiredItems.get("diplopiaPresentYN");                
        adequateAdjustmentYN =            (String)      requiredItems.get("adequateAdjustmentYN");             
        prism =                           (Boolean)     requiredItems.get("prism");                            
        patch =                           (Boolean)     requiredItems.get("patch");                            
        impairmentEvidence =              (String)      requiredItems.get("impairmentEvidence");               
        diagnosisCausingDecline =         (String)      requiredItems.get("diagnosisCausingDecline");          
        alzheimers =                      (Boolean)     requiredItems.get("alzheimers");                       
        lewyBodyDementia =                (Boolean)     requiredItems.get("lewyBodyDementia");                 
        parkinsonsWDementia =             (Boolean)     requiredItems.get("parkinsonsWDementia");              
        picksDiseaseOrComplex =           (Boolean)     requiredItems.get("picksDiseaseOrComplex");            
        vascularDementia =                (Boolean)     requiredItems.get("vascularDementia");                 
        conductedScreening =              (String)      requiredItems.get("conductedScreening");               
        mocaScore =                       (Long)        requiredItems.get("mocaScore");                        
        gdsScore =                        (Long)        requiredItems.get("gdsScore");                         
        trailsB =                         (Long)        requiredItems.get("trailsB");                          
        contributingFactors =             (String)      requiredItems.get("contributingFactors");              
        education =                       (Boolean)     requiredItems.get("education");                        
        hearingLoss =                     (Boolean)     requiredItems.get("hearingLoss");                      
        infection =                       (Boolean)     requiredItems.get("infection");                        
        thyroidDeficiencyOrExcess =       (Boolean)     requiredItems.get("thyroidDeficiencyOrExcess");        
        historyDebility =                 (String)      requiredItems.get("historyDebility");                  
        lossStrength =                    (Boolean)     requiredItems.get("lossStrength");                     
        romLoss =                         (Boolean)     requiredItems.get("romLoss");                          
        fatigue =                         (Boolean)     requiredItems.get("fatigue");                          
        lossextremityFlexion =            (Boolean)     requiredItems.get("lossextremityFlexion");             
        lossjointMobility =               (Boolean)     requiredItems.get("lossjointMobility");                
        losstrunkMobility =               (Boolean)     requiredItems.get("losstrunkMobility");                
        amputation =                      (Boolean)     requiredItems.get("amputation");                       
        rightArmAboveElbow =              (Boolean)     requiredItems.get("rightArmAboveElbow");               
        rightArmBelowElbow =              (Boolean)     requiredItems.get("rightArmBelowElbow");               
        leftArmAboveElbow =               (Boolean)     requiredItems.get("leftArmAboveElbow");                
        leftArmBelowElbow =               (Boolean)     requiredItems.get("leftArmBelowElbow");                
        rightLegBelowKnee =               (Boolean)     requiredItems.get("right Leg Below Knee");             
        leftLegBelowKnee =                (Boolean)     requiredItems.get("leftLegBelowKnee");                 
        rightLegAboveKnee =               (Boolean)     requiredItems.get("rightLegAboveKnee");                
        leftLegAboveKnee =                (Boolean)     requiredItems.get("leftLegAboveKnee");                 
        prosthesis =                      (String)      requiredItems.get("prosthesis");                       
        historychronicCondition =         (String)      requiredItems.get("historychronicCondition");          
        rheumatoidArthritis =             (Boolean)     requiredItems.get("rheumatoidArthritis");              
        osteoarthritis =                  (Boolean)     requiredItems.get("osteoarthritis");                   
        degenerativeDiscDisease =         (Boolean)     requiredItems.get("degenerativeDiscDisease");          
        permanentSpineInjury =            (Boolean)     requiredItems.get("permanentSpineInjury");             
        adaptiveEquipmentYN =             (String)      requiredItems.get("adaptiveEquipmentYN");              
        spinnerKnob =                     (Boolean)     requiredItems.get("spinnerKnob");                      
        handControls =                    (Boolean)     requiredItems.get("handControls");                     
        vehicleModifications =            (Boolean)     requiredItems.get("vehicleModifications");             
        yes =                             (Boolean)     requiredItems.get("yes");                              
        recommendFollowUp =               (Boolean)     requiredItems.get("recommendFollowUp");                
        recommendRoadTest =               (Boolean)     requiredItems.get("recommendRoadTest");                
        no =                              (Boolean)     requiredItems.get("no");                               
        OneYear =                         (Boolean)     requiredItems.get("1Year");                            
        TwoYears =                        (Boolean)     requiredItems.get("2Years");                           
        detailsCondition =                (String)      requiredItems.get("detailsCondition");                 
        attached =                        (Boolean)     requiredItems.get("attached");                         
        referralCompleted =               (Boolean)     requiredItems.get("referralCompleted");                
        referralRequired =                (Boolean)     requiredItems.get("referralRequired");   
        
        licenceRestrictionsCombined = combineListItems(licenceRestrictions, false);  
        acuityDiagnosisCombined = combineListItems(acuityDiagnosis, true);
        diagnosisVisualFieldImpairmentCombined = combineListItems(diagnosisVisualFieldImpairment, true);
	}
	
	private String combineListItems(List<String> items, boolean convertFromCamelCase) {
		StringBuilder sb = new StringBuilder();
		while(items.size() > 0) {
			String item = items.remove(0);
			if (convertFromCamelCase) {
				item = fromCC(item);
			}
			sb.append(item).append(items.size() > 0 ? ", " : "");		
		}
		return sb.toString();
	}
		
	private String fromCC(String cc){
		String s = StringUtils.capitalize(StringUtils.join(StringUtils.splitByCharacterTypeCamelCase(cc), " "));
		return s;
	}
}
