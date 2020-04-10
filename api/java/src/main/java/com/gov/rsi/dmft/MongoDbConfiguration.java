package com.gov.rsi.dmft;

import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.mongodb.config.AbstractMongoClientConfiguration;

import com.mongodb.ConnectionString;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;

/**
 * Configures the MongoDB connection with values from environment variables.
 * The following environment variables are examined:
 * <ul>
 * <li>MONGODB_HOST        defaults to localhost</li>
 * <li>MONGODB_PORT        defaults to 27017</li>
 * <li>MONGODB_DATABASE    defaults to test</li>
 * <li>MONGODB_USER        optional (if specified, both user and password are required)</li>
 * <li>MONGODB_PASSWORD    optional (if specified, both user and password are required)</li>
 * </ul>
 * <p>
 * Use of this class for configuration is disabled during the Maven Test phase,
 * when an embedded database is used 
 */
@Configuration
@ConditionalOnProperty(
	    value="SIMULATE_MONGO", 
	    havingValue = "false", 
	    matchIfMissing = true)
public class MongoDbConfiguration extends AbstractMongoClientConfiguration {

	private static Logger log = LoggerFactory.getLogger(MongoDbConfiguration.class);

	private Map<String, String> env;
	
	private String host;
	private String port;
	private String database;
	private String user;
	private String password;
	
	@Override
	public String getDatabaseName() {
		if (env == null) {
			load();
		}
		return database;
	}

	@Override
	@Bean
	public MongoClient mongoClient() {

		log.info("Configuring connection to MongoDB");
		if (env == null) {
			load();
		}
		
		StringBuilder sb = new StringBuilder("mongodb://");
		if (user != null) {
			sb.append(user).append(':').append(password).append('@');
		}
		sb.append(host).append(':').append(port);

		if (!database.equals("test")){
			// Must append the database name for authentication
			sb.append('/').append(database);
		}

		log.info("Connecting to MongoDB at " + sb.toString());

		ConnectionString cs = new ConnectionString(sb.toString());
		MongoClient client = MongoClients.create(cs);
		
		return client;
	}
	
	private void load() {
		env = System.getenv();

		host = env.getOrDefault("MONGODB_HOST", "localhost");
		log.debug("MONGODB_HOST " + host);

		port = env.getOrDefault("MONGODB_PORT", "27017");
		log.debug("MONGODB_PORT " + port);

		database = env.getOrDefault("MONGODB_DATABASE", "test");
		log.debug("MONGODB_DATABASE " + database);

		user = env.get("MONGODB_USER");
		log.debug("MONGODB_USER " + user);

		password = env.get("MONGODB_PASSWORD");
		log.debug("MONGODB_PASSWORD " + password);

		user = password == null ? null : user;
		password = user == null ? null : password;
	}
}