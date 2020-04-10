package com.gov.rsi.dmft.pdf;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.CharBuffer;
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

/**
 * Backer for the dmer.jasper report template. This class also provides a 
 * main method to invoke the compliation of the .jrxml report definition
 * file into the .jasper template
 */
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
    
    public String       	patientName;
    public String       	patientAddress;
    public String       	patientPhone;
    public String       	patientGender;
    public Date         	patientBirthDate;
    public String       	practitionerName;
    public String       	practitionerAddress;
    public String       	practitionerPhone;
    

    private Map<String, Object> requiredItems;
    private String				json;
	
	private static final String PARAM_BC_LOGO_GRAPHIC		= "BC_LOGO";
	private static final String PARAM_BOX_GRAPHIC			= "BOX";
	private static final String PARAM_BOX_CHECKED_GRAPHIC	= "BOX_CHECKED";
	
	private static final String FILE_BC_LOGO_GRAPHIC		= "BCLogo.png";
	private static final String FILE_BOX_GRAPHIC			= "Box.png";
	private static final String FILE_BOX_CHECKED_GRAPHIC	= "Box-Checked.png";
	
	private static Logger log = LoggerFactory.getLogger(DmerBacker.class);

	/**
	 * Entry point to compile the .jrxml file, or to generate the pdf from a sample eDMER
	 * @param args if present, arg[0] specifies the path to an eDMER JSON file relative to 
	 *             the execution folder, i.e. the system "user.dir" property
	 */
	public static void main(String[] args) {

		String jsonFileName = args.length == 0 ? null : args[0];
		if (jsonFileName == null) {
			(new DmerBacker()).compile("dmer");
		}
		else {
			String jsonPath = System.getProperty("user.dir");
			jsonPath = jsonPath.substring(0, jsonPath.lastIndexOf("\\") + 1) + jsonFileName;

			
			try (OutputStream os = new FileOutputStream(
					new File(samplesFolder() + "dmer.pdf"))){
				String json = readFile(jsonPath);
				DmerBacker backer = new DmerBacker(json);
				backer.initialize();
				backer.generatePdf(os);				
			}
			catch (IOException | JRException e) {
				System.out.println(e);
			}
		}
	}
	
	private static String readFile(String path) throws IOException {
		File file = new File(path);
		BufferedReader reader = new BufferedReader(new FileReader(file));
		
		// file size (byte count) will always exceed string length (char count)
		CharBuffer buffer = CharBuffer.allocate((int)file.length());
		reader.read(buffer);
		
		return buffer.rewind().toString();
	}
	
	public DmerBacker() {}
	
	/**
	 * Constructor
	 * @param json the eDMER JSON document which is to populate the generated PDF
	 */
	public DmerBacker(String json) {
		initialize();
		this.json = json;
	}
	
	/**
	 * Generates the PDF representation of the eDMER
	 * @param os stream to which the PDF is to be written
	 * @throws IOException if I/O errors occur
	 * @throws JRException if the PDF generation fails
	 */
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
        physician =                       (Boolean)     fixBool(requiredItems.get("physician"));                        
        np =                              (Boolean)     fixBool(requiredItems.get("np"));                               
        attachedPatient =                 (Boolean)     fixBool(requiredItems.get("attachedPatient"));                  
        walkInClinic =                    (Boolean)     fixBool(requiredItems.get("walkInClinic"));                     
        locum =                           (Boolean)     fixBool(requiredItems.get("locum"));                            
        specialist =                      (Boolean)     fixBool(requiredItems.get("specialist"));                       
        columns7MedicalIssued1 =          (Date)        requiredItems.get("columns7MedicalIssued1");           
        patientDL1 =                      (Long)        requiredItems.get("patientDL1");                   
        reasonForExam2 =           fromCC((String)      fixString(requiredItems.get("reasonForExam2")));                   
        licenceClass2 =            fromCC((String)      fixString(requiredItems.get("licenceClass2")));                    
        licenceRestrictions =             (List<String>)requiredItems.get("licenceRestrictions");              
        driverConsentDate =               (Date)        requiredItems.get("driverConsentDate");                
        leftUncorrected =                 (String)      fixString(requiredItems.get("leftUncorrected"));                  
        rightUncorrected =                (String)      fixString(requiredItems.get("rightUncorrected"));                 
        bothUncorrected =                 (String)      fixString(requiredItems.get("bothUncorrected"));                  
        leftCorrected =                   (String)      fixString(requiredItems.get("leftCorrected"));                    
        rightCorrected =                  (String)      fixString(requiredItems.get("rightCorrected"));                   
        bothCorrected =                   (String)      fixString(requiredItems.get("bothCorrected"));                  
        acuityLoss =                      (String)      fixString(requiredItems.get("AcuityLoss"));                       
        acuityLossProgressiveYN =         (String)      fixString(requiredItems.get("AcuityLossProgressiveYN"));          
        acuityDiagnosis =                 (List<String>)requiredItems.get("AcuityDiagnosis");                  
        AbnormalVFC =                     (String)      fixString(requiredItems.get("AbnormalVFC"));                      
        diagnosisVisualFieldImpairment =  (List<String>)requiredItems.get("DiagnosisVisualFieldImpairment");   
        dateOfOnset =                     (Date)        requiredItems.get("dateOfOnset");                      
        diplopiaPresentYN =               (String)      fixString(requiredItems.get("diplopiaPresentYN"));                
        adequateAdjustmentYN =            (String)      fixString(requiredItems.get("adequateAdjustmentYN"));             
        prism =                           (Boolean)     fixBool(requiredItems.get("prism"));                            
        patch =                           (Boolean)     fixBool(requiredItems.get("patch"));                            
        impairmentEvidence =              (String)      fixString(requiredItems.get("impairmentEvidence"));               
        diagnosisCausingDecline =         (String)      fixString(requiredItems.get("diagnosisCausingDecline"));          
        alzheimers =                      (Boolean)     fixBool(requiredItems.get("alzheimers"));                       
        lewyBodyDementia =                (Boolean)     fixBool(requiredItems.get("lewyBodyDementia"));                 
        parkinsonsWDementia =             (Boolean)     fixBool(requiredItems.get("parkinsonsWDementia"));              
        picksDiseaseOrComplex =           (Boolean)     fixBool(requiredItems.get("picksDiseaseOrComplex"));            
        vascularDementia =                (Boolean)     fixBool(requiredItems.get("vascularDementia"));                 
        conductedScreening =              (String)      fixString(requiredItems.get("conductedScreening"));               
        mocaScore =                       (Long)        requiredItems.get("mocaScore");                        
        gdsScore =                        (Long)        requiredItems.get("gdsScore");                         
        trailsB =                         (Long)        requiredItems.get("trailsB");                          
        contributingFactors =             (String)      fixString(requiredItems.get("contributingFactors"));              
        education =                       (Boolean)     fixBool(requiredItems.get("education"));                        
        hearingLoss =                     (Boolean)     fixBool(requiredItems.get("hearingLoss"));                      
        infection =                       (Boolean)     fixBool(requiredItems.get("infection"));                        
        thyroidDeficiencyOrExcess =       (Boolean)     fixBool(requiredItems.get("thyroidDeficiencyOrExcess"));        
        historyDebility =                 (String)      fixString(requiredItems.get("historyDebility"));                  
        lossStrength =                    (Boolean)     fixBool(requiredItems.get("lossStrength"));                     
        romLoss =                         (Boolean)     fixBool(requiredItems.get("romLoss"));                          
        fatigue =                         (Boolean)     fixBool(requiredItems.get("fatigue"));                          
        lossextremityFlexion =            (Boolean)     fixBool(requiredItems.get("lossextremityFlexion"));             
        lossjointMobility =               (Boolean)     fixBool(requiredItems.get("lossjointMobility"));                
        losstrunkMobility =               (Boolean)     fixBool(requiredItems.get("losstrunkMobility"));                
        amputation =                      (Boolean)     fixBool(requiredItems.get("amputation"));                       
        rightArmAboveElbow =              (Boolean)     fixBool(requiredItems.get("rightArmAboveElbow"));               
        rightArmBelowElbow =              (Boolean)     fixBool(requiredItems.get("rightArmBelowElbow"));               
        leftArmAboveElbow =               (Boolean)     fixBool(requiredItems.get("leftArmAboveElbow"));                
        leftArmBelowElbow =               (Boolean)     fixBool(requiredItems.get("leftArmBelowElbow"));                
        rightLegBelowKnee =               (Boolean)     fixBool(requiredItems.get("right Leg Below Knee"));             
        leftLegBelowKnee =                (Boolean)     fixBool(requiredItems.get("leftLegBelowKnee"));                 
        rightLegAboveKnee =               (Boolean)     fixBool(requiredItems.get("rightLegAboveKnee"));                
        leftLegAboveKnee =                (Boolean)     fixBool(requiredItems.get("leftLegAboveKnee"));                 
        prosthesis =                      (String)      fixString(requiredItems.get("prosthesis"));                       
        historychronicCondition =         (String)      fixString(requiredItems.get("historychronicCondition"));          
        rheumatoidArthritis =             (Boolean)     fixBool(requiredItems.get("rheumatoidArthritis"));              
        osteoarthritis =                  (Boolean)     fixBool(requiredItems.get("osteoarthritis"));                   
        degenerativeDiscDisease =         (Boolean)     fixBool(requiredItems.get("degenerativeDiscDisease"));          
        permanentSpineInjury =            (Boolean)     fixBool(requiredItems.get("permanentSpineInjury"));             
        adaptiveEquipmentYN =             (String)      fixString(requiredItems.get("adaptiveEquipmentYN"));              
        spinnerKnob =                     (Boolean)     fixBool(requiredItems.get("spinnerKnob"));                      
        handControls =                    (Boolean)     fixBool(requiredItems.get("handControls"));                     
        vehicleModifications =            (Boolean)     fixBool(requiredItems.get("vehicleModifications"));             
        yes =                             (Boolean)     fixBool(requiredItems.get("yes"));                              
        recommendFollowUp =               (Boolean)     fixBool(requiredItems.get("recommendFollowUp"));                
        recommendRoadTest =               (Boolean)     fixBool(requiredItems.get("recommendRoadTest"));                
        no =                              (Boolean)     fixBool(requiredItems.get("no"));                               
        OneYear =                         (Boolean)     fixBool(requiredItems.get("1Year"));                            
        TwoYears =                        (Boolean)     fixBool(requiredItems.get("2Years"));                           
        detailsCondition =                (String)      fixString(requiredItems.get("detailsCondition"));                 
        attached =                        (Boolean)     fixBool(requiredItems.get("attached"));                         
        referralCompleted =               (Boolean)     fixBool(requiredItems.get("referralCompleted"));                
        referralRequired =                (Boolean)     fixBool(requiredItems.get("referralRequired"));   

        patientName =                   (String)fixString(requiredItems.get("patientName"));
        patientAddress =                (String)fixString(requiredItems.get("patientAddress"));
        patientPhone =                  (String)fixString(requiredItems.get("patientPhone"));
        patientGender =                 (String)fixString(requiredItems.get("patientGender"));
        patientBirthDate =              (Date)requiredItems.get("patientBirthDate");
        practitionerName =              (String)fixString(requiredItems.get("practitionerName"));
        practitionerAddress =           (String)fixString(requiredItems.get("practitionerAddress"));
        practitionerPhone =             (String)fixString(requiredItems.get("practitionerPhone"));        
        
        licenceRestrictionsCombined = combineListItems(licenceRestrictions, false);  
        acuityDiagnosisCombined = combineListItems(acuityDiagnosis, true);
        diagnosisVisualFieldImpairmentCombined = combineListItems(diagnosisVisualFieldImpairment, true);
	}
	
	private String combineListItems(List<String> items, boolean convertFromCamelCase) {
		if (items == null) {
			return "";
		}
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
	
	private Object fixBool(Object o) {
		return o == null ? false: o;
	}
	private Object fixString(Object o) {
		return o == null ? "": o;
	}
}
