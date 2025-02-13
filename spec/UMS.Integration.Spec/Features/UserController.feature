Feature: User Registration
In order to manage user account
I want to register new users and perform other account management operations

    Background:
        Given the cache exists with key "johnsmith@example.com"
        And the cache contains value "123456"

    Scenario: Register user with valid data
        When a user registers with valid data
        Then the response status code should be 201 Created

    Scenario: Register user with existing email
        When a user registers with existing email
        Then the response status code should be 400 BadRequest
        And the response should contain "User with Email mikemill@example.com already exists"

    Scenario: Register user with invalid OTP
        When a user registers with invalid OTP
        Then the response status code should be 400 BadRequest
        And the response should contain "Invalid OTP."

    Scenario: Retrieve user data using WhoAmI endpoint
        Given a user is authenticated with ID "1"
        When the user accesses the WhoAmI endpoint
        Then the WhoAmI response status code should be 200 OK
        And the response should contain user data:
          | FirstName | LastName | EmailAddress        |
          | Jane      | Doe      | janedoe@example.com |
          
    Scenario: Unauthorized access to WhoAmI endpoint
        Given a user is unauthenticated
        When the user accesses the WhoAmI endpoint
        Then the response status code should be 401 Unauthorized
        
    Scenario: Authenticated user resets password
        Given a user is authenticated with ID "2"
        When the user resets the password with valid data
        Then the reset password response status code should be 200 OK
        
    Scenario: Authenticated user resets password with invalid OTP
        Given a user is authenticated with ID "2"
        When the user resets the password with invalid OTP
        Then the response status code should be 400 BadRequest
        And the response should contain "Invalid OTP."
        
    Scenario: Unauthenticated user attempts to reset password
        Given a user is unauthenticated
        When the user attempts to reset the password
        Then the response status code should be 401 Unauthorized
        
    Scenario: Authenticated user changes password
        Given a user is authenticated with ID "2"
        When the user changes the password with valid data
        Then the change password response status code should be 200 OK
        
    Scenario: Unauthenticated user attempts to change password
        Given a user is unauthenticated
        When the user attempts to change the password
        Then the response status code should be 401 Unauthorized
        
    Scenario: Authenticated user changes password with invalid old password
        Given a user is authenticated with ID "2"
        When the user changes the password with invalid old password
        Then the response status code should be 400 BadRequest
        And the response should contain "Invalid password."