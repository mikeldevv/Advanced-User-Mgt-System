Feature: Sending OTP via Email

    Scenario: Sending OTP with valid data returns 200 OK
        Given the following request data:
          | EmailAddress     | Purpose      |
          | test@example.com | Registration |

        When I send the OTP via email
        Then the response status code should be 200 OK