Feature: RedisCache

    Scenario: Checking existence of an existing key
        Given a Redis cache with a key "testKey" and a time-to-live of 5 minutes
        When I check the existence of the key "testKey"
        Then the result should indicate that the key exists and provide the time-to-live of 5 minutes
        
    Scenario: Checking existence of a non-existent key
        Given a Redis cache without the key "nonExistentKey"
        When I check the existence of the key "nonExistentKey"
        Then the result should indicate that the key does not exist and the time-to-live should be null
        
    Scenario: Reading a valid value from the cache
        Given a Redis cache with a key "testKey", a value "testValue", and a time-to-live of 5 minutes
        When I read the key "testKey"
        Then the result should indicate a valid value "testValue" and an expiration time of 5 minutes
        
    Scenario: Reading a non-existent key
        Given a Redis cache without the key "nonExistentKey"
        When I read the key "nonExistentKey"
        Then the result should be null and the expiration time should be zero
        
    Scenario: Deleting a key
        Given a Redis cache
        When I delete the key "testKey"
        Then the key "testKey" should be deleted from the cache
        
    Scenario: Writing to cache with specified TTL
        Given a Redis cache
        When I write the value "testValue" to the key "testKey" with a TTL of 10 minutes
        Then the value "testValue" should be written to the key "testKey" in the cache with the specified TTL

    Scenario: Writing to cache with preserved TTL
        Given a Redis cache with an expiring key "expiringKey" and an existing TTL of 5 minutes
        When I write the value "testValue" to the key "expiringKey" while preserving the TTL
        Then the value "testValue" should be written to the key "expiringKey" in the cache with the preserved TTL
