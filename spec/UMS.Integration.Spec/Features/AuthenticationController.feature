Feature: AuthenticationController
In order to secure my application
As a user
I want to be able to log in

    Scenario: Logging in with valid data should return 200 OK
        When I log in with email "janedoe@example.com" and password "jane123"
        Then I should receive a 200 response

    Scenario: Logging in with invalid data should return 400 Bad Request
        When I log in with email "invalid@example.com" and password "invalid"
        Then I should receive a 400 Bad Request response containing "Invalid Email Address or Password"