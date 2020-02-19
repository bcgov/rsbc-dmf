# DMFT API

The Driver Medical Fitness Test API provides a way for Physicians to submit an eDMER.

## Setup

Create a `.env` file and populate it with the following;

```conf
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
MONGODB_DATABASE=dmft
ConnectionStrings__Dmer=mongodb://[username]:[password]@database:27017/dmft
```

Enter the [username] you created when initializing the DB.

Enter the user [password] you set when initializing the DB.

Make sure the 'database' and 'port' are also configured the same as your docker configuration.
