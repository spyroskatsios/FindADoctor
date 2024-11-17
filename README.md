# Find A Doctor

### An application for doctors and patients to connect and schedule appointments.

In this "demo" application you can explore concepts and tools like: 
<br/>
<br/>
Domain Driven Design, Eventual Consistency, Bounded Contexts, Aggregates, Domain & Integration Events, Clean Architecture, CQRS, Microservices, JWT Authentication with Refresh Tokens, Asynchronous Messaging, 
RabbitMQ, Open Telemetry, Unit Testing, Integration Testing, Subcutaneous Testing, Test Containers, Jaeger, Loki, Grafana, Prometheus, GitHub Actions and more.
<br/>
<br/>
All services are contained within the same solution for easier navigation, but they can be deployed independently.

## Services:

## Identity Service

The Identity Service is responsible for user management and authentication. It provides JWT tokens and refresh tokens. Just to keep things simple
this service does not communicate with the other services; users must be manually added to other services.

### Endpoints:

<details>
 <summary><code>POST</code> <code>IDENTITY_URL/identity/register-doctor</code> <code>(Registers a Doctor in the Identity Service)</code></summary>

```
{
    "userName" : "doctor", 
    "email" : "doctor@doctor.com",
    "password" : "MyPass1!"
}
```
</details>

<details>
 <summary><code>POST</code> <code>IDENTITY_URL/identity/register-patient</code> <code>(Registers a Patient in the Identity Service)</code></summary>

```
{
  "userName": "patient",
  "email": "patient@patient.com",
  "password": "MyPass1!"
}
```
</details>

<details>
 <summary><code>POST</code> <code>IDENTITY_URL/identity/login</code> <code>(Returns the JWT and the refresh Token)</code></summary>

```
{
  "userName": "patient",
  "password": "MyPass1!"
}
```
</details>

<details>
 <summary><code>POST</code> <code>IDENTITY_URL/identity/refresh-token</code> <code>(Refreshes the JWT)</code></summary>

```
{
  "token": "",
  "refreshToken": ""
}
```
</details>

## Doctors Service

The Doctors Service is responsible for managing doctors and their offices. A doctor has a Subscription, and based on it can add a different amount of Offices.

### Endpoints:

<details>
 <summary><code>POST</code> <code>DOCTORS_URL/doctors</code> <code>(Creates a Doctor)</code></summary>

```
{
  "firstName": "Doctor",
  "lastName": "Dre",
  "speciality": 2
}
```
</details>

<details>
 <summary><code>POST</code> <code>DOCTORS_URL/subscriptions</code> <code>(Creates a Subscription)</code></summary>

```
{
  "subscriptionType": 2
}
```
</details>

<details>
 <summary><code>POST</code> <code>DOCTORS_URL/doctors/{doctorId}/offices</code> <code>(Creates an Office)</code></summary>

```
{
  "state": "",
  "city": "",
  "street": "",
  "streetNumber": "",
  "zipCode": ""
}
```
</details>
<br/>
 <summary><code>GET</code> <code>DOCTORS_URL/doctors/{doctorId}</code> <code>(Returns a Doctor)</code></summary>
 <summary><code>GET</code> <code>DOCTORS_URL/doctors/</code> <code>(Returns the Doctors matching the search criteria)</code></summary>
 <summary><code>GET</code> <code>DOCTORS_URL/subscriptions/{subscriptionId}</code> <code>(Returns a Subscription)</code></summary>
 <summary><code>GET</code> <code>DOCTORS_URL/offices/{officeId}</code> <code>(Returns an Office)</code></summary>
 <summary><code>GET</code> <code>DOCTORS_URL/doctors/{doctorId}/offices</code> <code>(Returns all the Offices of a Doctor)</code></summary>
<br/>
 <summary><code>DELETE</code> <code>DOCTORS_URL/offices/{officeId}</code> <code>(Deletes an Office)</code></summary>

## Appointments Service

The Appointments Service is responsible for managing appointments. This is also where the Patients live. Doctors can add schedules to their Offices, and based on it the Patients can schedule appointments.

### Endpoints:

<details>
 <summary><code>POST</code> <code>APPOINTMENTS_URL/patients</code> <code>(Creates a Patient)</code></summary>

```
{
}
```
</details>

<details>
 <summary><code>POST</code> <code>APPOINTMENTS_URL/offices/{officeId}/schedule</code> <code>(Adds a Schedule to the Office)</code></summary>

```
{
  "workingCalendar": [
    {
      "date": "2024-12-06T00:00:00",
      "timeRange": {
        "start": "2024-12-06T10:00:00",
        "end": "2024-12-06T18:00:00"
      }
    }
  ]
}
```
</details>

<details>
 <summary><code>POST</code> <code>APPOINTMENTS_URL/offices/{officeId}/appointments</code> <code>(Books an Appointment)</code></summary>

```
{
  "dateTime": "2024-12-06T10:00:00"
}
```
</details>

<details>
 <summary><code>POST</code> <code>APPOINTMENTS_URL/appointments/{appointmentId}/cancel</code> <code>(Cancels an Appointment)</code></summary>

```
{
}
```
</details>
<br/>
 <summary><code>GET</code> <code>APPOINTMENTS_URL/appointments/{appointmentId}</code> <code>(Returns an Appointment)</code></summary>

## Example Flow:

When a doctor deletes an office, the office is deleted (soft delete) in the Doctors Service.
An Integration Event is published, triggering the Appointments Service to delete the office and cancel all related appointments through a Domain Event.

## Docker Compose
A Docker Compose file is available in the project root to run RabbitMQ, Jaeger, Grafana and the Open Telemetry Collector.

## Open Telemetry
The application utilizes OpenTelemetry for comprehensive observability, including distributed tracing, metrics, and logging. 

## GitHub Actions

GitHub Actions are configured to run all tests. Workflows can be found in the `.github/workflows` folder.

## Postman Collection
A Postman collection is available in the project root for testing the application.

