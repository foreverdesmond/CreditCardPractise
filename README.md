# CreditCardPractise


A .NET 8 Web API for validating credit card numbers using the Luhn algorithm. This project demonstrates professional coding standards, dependency injection, API validation, error handling, and test automation.

## Features

	•	Validate credit card numbers using the Luhn algorithm.
	•	Modular structure for scalability (Utilities, Services, Factories).
	•	Comprehensive error handling using middleware and NLog.
	•	API validation using filters.
	•	Fully tested with unit and integration tests.

 ## API Documentation

 ### Base URL
 http://localhost:7021/api/creditcard

 ### Endpoints
 
 #### 1. Validate Credit Card Number
 URL: GET /validate/card-number

 Description: Validates the provided credit card number using the Luhn algorithm.

 Request Parameters:
	•	cardNumber (string, required): A numeric string representing the credit card number. It must contain 13 to 16 digits.

 Responses:
	•	200 OK: Returns the card number and its validity status.
	•	400 Bad Request: If the card number is invalid or does not match the required format.
	•	500 Internal Server Error: If the credit card service is unavailable.

 
 Example Request:
 
 GET /api/creditcard/validate/card-number?cardNumber=4532015112830366 HTTP/1.1
 Host: localhost:7021

 Example Response (200):
 ```
 {
    "CardNumber": "4532015112830366",
    "IsValid": true
 }
 ```

 Example Response (400):
 ```
 {
    "errors": "Card number must be a 13 to 16-digit number."
 }
 ```

 Example Response (500):
 ```
 {
    "errors": "CreditCard service is not available."
 }
 ```




 #### 2. Validate Credit Card Details
 URL: POST /validate/credit-card

 Description: Validates a detailed credit card DTO for its validity.

 Request Body:
	•	CreditCardDto (JSON, required): A JSON object containing detailed credit card information:
	•	cardNumber (string): A numeric string representing the credit card number.
	•	cardHolderName (string): The name of the credit card holder.
	•	expiryMonth (integer): The expiry month of the card.
	•	expiryYear (integer): The expiry year of the card.
	•	cvv (string): The card verification value (CVV).
	•	issuer (string): The name of the credit card issuer.


 Responses:
	•	200 OK: Returns the card number and its validity status.
	•	400 Bad Request: If the credit card information is invalid or does not match the required format.
	•	500 Internal Server Error: If the credit card service is unavailable.

 Example Request:
 ```
 POST /api/creditcard/validate/credit-card HTTP/1.1
 Host: localhost:7021
 Content-Type: application/json

 {
   "cardNumber": "4532015112830366",
   "cardHolderName": "John Doe",
   "expiryMonth": 12,
   "expiryYear": 2025,
   "cvv": "123",
   "issuer": "Visa"
 }
 ```

 Example Response (200):
 ```
 {
    "CardNumber": "4532015112830366",
    "IsValid": true
 }
```

 Example Response (400):
 ```
 {
   "errors": "Expiry month must be between 1 and 12."
 }
```

 Example Response (500):
 ```
 {
    "errors": "CreditCard service is not available."
 }
```

 ## Project Setup

 Requirements
 
  •	.NET 8 SDK
	•	Visual Studio or VS Code
	•	A web browser for testing

	

 Steps to Run
 
 1.	Clone the repository:

 ```
 git clone https://github.com/foreverdesmond/CreditCardPractise.git
 cd CreditCardPractise
```

 2.	Navigate to the API project directory:

  ```
 cd CreditCard.API
```
  
 3.	Run the API:

 ```
 cd CreditCardPractise
 dotnet run
```

 4.	Access Swagger documentation in your browser:
```
 http://localhost:5000/swagger
```

## Project Structure
```
CreditCardPractise/
├── CreditCard.API/         # API Project
│   ├── Controllers/        # API Controllers
│   ├── Filters/            # Validation Filters
│   ├── ErrorHandling/      # Middleware for Exception Handling
│   └── Program.cs          # Application Entry Point
├── CreditCard.BusinessLogic/ # Business Logic
│   ├── Services/           # Services for Business Operations
│   ├── Utilities/          # Luhn Algorithm Implementation
│   └── Factories/          # Factory Design for Service Creation
├── CreditCard.Models/      # Data Models and DTOs
│   ├── DTOs/               # Data Transfer Objects
│   ├── Models/             # Database Models
│   └── Mapper/             # AutoMapper Profiles
├── CreditCard.Test/        # Test Project
│   ├── UnitTests/          # Unit Tests for API Components
│   └── IntegrationTests/   # Integration Tests with WebApplicationFactory
```
