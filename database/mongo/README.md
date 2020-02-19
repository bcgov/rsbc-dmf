# Mongo DB

## OpenShift Container

- https://docs.openshift.com/container-platform/3.11/using_images/db_images/mongodb.html

## Setup

Create a `.env` file to support Docker Compose.

```conf
MONGODB_USER=[username]
MONGODB_PASSWORD=[user password]
MONGODB_DATABASE=dmft
MONGODB_ADMIN_PASSWORD=[admin password]
```

Enter the [username] you want to use to access the DB.

Enter the [user password] you want to use to access the DB.

Enter the [admin password] you want to use for admin access to the DB.
