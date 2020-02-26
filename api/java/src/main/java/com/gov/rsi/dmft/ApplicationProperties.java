package com.gov.rsi.dmft;

import java.io.InputStream;
import java.util.Properties;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * A singleton holding configurable properties for the application. 
 * In this implementation the properties are stored in the "application.properties" 
 * file in the conventional "resources" folder in the project root. 
 * Future versions could incorporate e.g. a database or url source.
 */
public class ApplicationProperties {
	
	private static final Logger log = LoggerFactory.getLogger(ApplicationProperties.class.getName());
	
	private Properties	properties;
	
	private ApplicationProperties() {
		InputStream is = this.getClass().getClassLoader().getResourceAsStream("application.properties");
		properties = new Properties();
		try {
			properties.load(is);
		}
		catch (Exception e) {
			log.error("Unable to load application.properties file " + e);
		}
	}

    private static class SingletonHelper{
        private static final ApplicationProperties INSTANCE = new ApplicationProperties();
    }
    
    public static Properties getProperties(){
        return SingletonHelper.INSTANCE.properties;
    }
    
    public static String get(String key) {
    	return (String)getProperties().get(key);
    }

}
