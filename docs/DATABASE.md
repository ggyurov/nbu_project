# Database scheme

#### Tables

###### Appointments

* Id - INT32 - Primary key (appointment unique identifier)
* UserId - NVARCHAR(450) - Foreign key to users table (the user that created the appointment)
* Date - DATETIME - (the date and time of the appointment)
* DoctorId (TODO: ??) - NVARCHAR(450) - Foreign key to users table (the doctor)
* CanceledById - NVARCHAR(450) (NULLABLE) - FK to users (the user that canceled the appointment)
* CanceledOn - DATETIME (NULLABLE) - (the date and time of the cancellation)

###### Ratings

* Id - INT32 - Primary key (rating row identifier)
* UserId - NVARCHAR(450) - FK to users (the rated user)
* ByUserId - NVARCHAR(450) - FK to users (rated by user)
* Value - BIT - (0 for `-`, 1 for `+`)

###### Comments

* Id - INT32 - Primary key (comment identifier)
* Text - NVARCHAR(256) - (comment text)
* ByUserId - NVARCHAR(450) - FK to users (the creator)
* UserId - NVARCHAR(450) (NULLABLE) - FK to users (if commented on doctor profile)
* EventId - INT32 (NULLABLE) - FK to events (if commented an event)

###### Events

* Id - INT32 - Primary key (event identifier)
* UserId - NVARCHAR(450) - (the creator)
* Title - NVARCHAR(128) - (event title)
* Text - NVARCHAR(256) - (event text)
* StartDate - DATETIME - (event start date and time)
* EndDate - DATETIME - (event end date and time)

###### Users (extended version the current IdentityUser table)

* Location - (multiple fields for the city and address of the doctor)
* 