# **TV Show Application**

This API is for managing a collection of TV Shows, by categorising them into genres. The API also allows you to leave reviews for those series.

## **Response Codes**
| Method | Possible Response Codes |
|-|-|
| GET | 200, 404 |
| POST | 201, 400, 404 |
| PATCH | 200, 404 |
| DELETE | 200, 404 |

## **User Endpoints**
---
## **GET User Info**
Returns some user info based on user id.
#### **URL Template**
https://{hostname}:{port}/api/user/{userId}
#### **Resource Information**
|||
| ----------- | ----------- |
| Requires Authentication | Yes |
| Response Formats | JSON |
| Rate Limited | No |
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| userId | yes | 32 bit integer user ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/user/1
#### **Example Response**
```
200
{
  "id": "1",
  "email": "john.doe@gmail.com",
  "reviews": [
    "/genre/1/series/1/review/1",
    "/genre/1/series/2/review/2"
  ]
}
```

## **POST Register**
Creates a new user using the provided email, password and role secret. Depending on given role secret you will be assigned a basic, poster or admin role.
#### **URL Template**
https://{hostname}:{port}/api/user/
#### **Resource Information**
|||
| ----------- | ----------- |
| Requires Authentication | No |
| Response Formats | JSON |
| Rate Limited | No |
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| email | yes | any email, must be in the format of "anything@anything" | "john.doe@gmail.com" |
| password | yes | any string | "this is a password" |
| roleSecret | yes, however any value will suffice | A secret to create the account as a part of a specific role | "anything" |
#### **Example Request**
POST https://{hostname}:{port}/api/user/
#### **Example Response**
```
200
{
  "password": "testpassword",
  "email": "john.doe@gmail.com",
  "roleSecret": "basic-user"
}
```

## **POST Login**
Given login details returns a refresh token (base64 string) and JWT access token
#### **URL Template**
https://{hostname}:{port}/api/user/token/
#### **Resource Information**
|||
| ----------- | ----------- |
| Requires Authentication | No |
| Response Formats | JSON |
| Rate Limited | No |
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| email | yes | any email, must be in the format of "anything@anything" | "john.doe@gmail.com" |
| password | yes | any string | "this is a password" |
#### **Example Request**
POST https://{hostname}:{port}/api/user/token/
#### **Example Response**
```
200
{
  "accessToken": "jwtaccesstoken",
  "refreshToken": "base64string",
}
```

## **POST Refresh Token**
Given access token and unexpired refresh token, will return new active refresh and access tokens
#### **URL Template**
https://{hostname}:{port}/api/user/token/refresh
#### **Resource Information**
|||
| ----------- | ----------- |
| Requires Authentication | No |
| Response Formats | JSON |
| Rate Limited | No |
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| accessToken | yes | must be valid JWT token | "validjwttoken" |
| refreshToken | yes | base64 string | "validrefreshtoken" |
#### **Example Request**
POST https://{hostname}:{port}/api/user/token/refresh
#### **Example Response**
```
200
{
  "accessToken": "jwtaccesstoken",
  "refreshToken": "base64string",
}
```

## **POST Revoke Token**
Takes user from given authentication JWT token and invalidates their refresh token. Access token will remain valid, however refreshing will not work since refresh token was invalidated.
#### **URL Template**
https://{hostname}:{port}/api/user/token/revoke
#### **Resource Information**
|||
| ----------- | ----------- |
| Requires Authentication | Yes |
| Response Formats | JSON |
| Rate Limited | No |
#### **Example Request**
POST https://{hostname}:{port}/api/user/token/revoke
#### **Example Response**
```
Status 200
```

## Genre Endpoints
|||
|-|-|
| Requires Authentication | Yes |
| Response Formats | JSON |
| Rate Limited | No |
---
## **GET Genre**
Returns genre based on id.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/genre/1
#### **Example Response**
```
200
{
    "id": "1",
    "name": "Horror",
    "description": "Very scary"
    "series": [
        "/genre/1/series/1",
        "/genre/1/series/2"
    ]
}
```

## **GET Genres**
Returns all genres.
#### **URL Template**
https://{hostname}:{port}/api/genre
#### **Example Request**
GET https://{hostname}:{port}/api/genre
#### **Example Response**
```
200
[
    {
        "id": "1",
        "name": "Horror",
        "description": "Very scary"
        "series": [
            "/genre/1/series/1",
            "/genre/1/series/2"
        ]
    },
    {
        ...
    }
]
```

## **POST Genre**
Creates genre.
#### **URL Template**
https://{hostname}:{port}/api/genre
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| name | yes | any string | "Action" |
| description | yes | any string | "description" |
#### **Example Request**
```
POST https://{hostname}:{port}/api/genre/6
{
    "name": "Testttttttt",
    "description": "Delete me"
}
```
#### **Example Response**
```
Location: https://{hostname}:{port}/api/genre/6

201
{
    "id": 6,
    "name": "Testttttttt",
    "description": "Delete me",
    "series": []
}
```

## **PATCH Genre**
Updates genre.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| description | yes | any string | "description" |
#### **Example Request**
```
PATCH https://{hostname}:{port}/api/genre/6
{
    "description": "new description"
}
```
#### **Example Response**
```
Status 200
```

## **DELETE Genre**
Deletes genre.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
#### **Example Request**
DELETE https://{hostname}:{port}/api/genre/6
#### **Example Response**
```
Status 200
```

## Series Endpoints
|||
|-|-|
| Requires Authentication | Yes |
| Response Formats | JSON |
| Rate Limited | No |
---

## **GET Series by id**
Returns series based on id.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/genre/1/series/2
#### **Example Response**
```
200
{
    "id": 2,
    "name": "Interstellar",
    "description": "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
    "coverImagePath": "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
    "directors": [
        "Christopher Nolan"
    ],
    "starringCast": [
        "Matthew McConaughey",
        "Anne Hathaway",
        "Jessica Chastain"
    ],
    "poster": {
        "href": "/user/1"
    },
    "genres": [
        "/genre/1",
        "/genre/2"
    ],
    "reviews": [
        "/genre/1/series/2/review/2",
        "/genre/1/series/2/review/4"
    ]
}
```

## **GET All Series**
Returns all series.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/genre/1/series
#### **Example Response**
```
200
[
    {
        "id": 2,
        "name": "Interstellar",
        "description": "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
        "coverImagePath": "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
        "directors": [
            "Christopher Nolan"
        ],
        "starringCast": [
            "Matthew McConaughey",
            "Anne Hathaway",
            "Jessica Chastain"
        ],
        "poster": {
            "href": "/user/1"
        },
        "genres": [
            "/genre/1",
            "/genre/2"
        ],
        "reviews": [
            "/genre/1/series/2/review/2",
            "/genre/1/series/2/review/4"
        ]
    }
]
```

## **POST Series**
Creates series.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| description | yes | any string | "name" |
| genreId | yes | 32 bit integer genre ID | 1 |
| description | yes | any string | "description" |
| coverImagePath | yes | any string | "invalid address" |
| directors | yes | string array | ["a", "b"] |
| starringCast | yes | string array | ["a", "b"] |
| poster | yes | user ID for poster | 1 |
| genres | yes | 32 bit integer array to specify which genres to link to, genre ID from route is automatically included | [1, 2, 3] |
#### **Example Request**
```
POST https://{hostname}:{port}/api/genre/6/series
{
    "name": "The test basic user",
    "description": "A frontiersman on a fur trading expedition in the 1820s fights for survival after being mauled by a bear and left for dead by members of his own hunting team.",
    "coverImagePath": "https://m.media-amazon.com/images/M/MV5BMDE5OWMzM2QtOTU2ZS00NzAyLWI2MDEtOTRlYjIxZGM0OWRjXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_.jpg",
    "directors": [
        "Alejandro G. Inarritu"
    ],
    "starringCast": [
        "Tom Hardy",
        "Leonardo Dicaprio",
        "Will Poulter"
    ],
    "poster": 1,
    "genres": []
}
```
#### **Example Response**
```
Location: https://{hostname}:{port}/api/genre/6/series/3

201
{
    "id": "3"
    "name": "The test basic user",
    "description": "A frontiersman on a fur trading expedition in the 1820s fights for survival after being mauled by a bear and left for dead by members of his own hunting team.",
    "coverImagePath": "https://m.media-amazon.com/images/M/MV5BMDE5OWMzM2QtOTU2ZS00NzAyLWI2MDEtOTRlYjIxZGM0OWRjXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_.jpg",
    "directors": [
        "Alejandro G. Inarritu"
    ],
    "starringCast": [
        "Tom Hardy",
        "Leonardo Dicaprio",
        "Will Poulter"
    ],
    "poster": 1,
    "genres": []
}
```

## **PATCH Series**
Updates series.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
| description | yes | any string | "description" |
| coverImagePath | yes | any string | "invalid address" |
| directors | yes | string array | ["a", "b"] |
| starringCast | yes | string array | ["a", "b"] |
#### **Example Request**
```
PATCH https://{hostname}:{port}/api/genre/6/series/3
{
    "description": "A m.",
    "coverImagePath": "https://m.media-amazon.com/images/M/MV5BMDE5OWMzM2QtOTU2ZS00NzAyLWI2MDEtOTRlYjIxZGM0OWRjXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_.jpg",
    "directors": [
        "Alejandro G. Inarritu"
    ],
    "starringCast": [
        "Tom Hardy",
        "Leonardo Dicaprio",
        "Will Poulter"
    ]
}
```
#### **Example Response**
```
Status 200
```

## **DELETE Series**
Deletes series.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
#### **Example Request**
DELETE https://{hostname}:{port}/api/genre/6/series/3
#### **Example Response**
```
Status 200
```

## Review Endpoints
|||
|-|-|
| Requires Authentication | Yes |
| Response Formats | JSON |
| Rate Limited | No |
---

## **GET Review**
Returns review based on id.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}/review/{reviewId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
| reviewId | yes | 32 bit integer series ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/genre/1/series/2/review/1
#### **Example Response**
```
200
{
    "id": 2,
    "postDate": "0001-01-01T00:00:00",
    "text": "This movie sucks",
    "rating": 1,
    "reviewedSeries": {
        "href": "/genre/1/series/2"
    },
    "reviewer": {
        "href": "/user/1"
    }
}
```

## **GET All Reviews**
Returns all reviews.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}/review
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
#### **Example Request**
GET https://{hostname}:{port}/api/genre/1/series/2/review
#### **Example Response**
```
200
[
    {
        "id": 2,
        "postDate": "0001-01-01T00:00:00",
        "text": "This movie sucks",
        "rating": 1,
        "reviewedSeries": {
            "href": "/genre/1/series/2"
        },
        "reviewer": {
            "href": "/user/1"
        }
    }
]
```

## **POST Review**
Creates review.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}/review
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
| text | Yes | any string | "Masterpiece" |
| rating | Yes | integer 1 to 10 | 10 |
| series | Yes | series id | 1 |
| user | Yes | ID for the user that is posting the review | 5 |
#### **Example Request**
```
POST https://{hostname}:{port}/api/genre/6/series/1/review
{
    "text": "Masterpiece",
    "rating": 10,
    "series": 1,
    "user": 5
}
```
#### **Example Response**
```
Location: https://{hostname}:{port}/api/genre/6/series/3/review/2

201
{
    "id": "2"
    "text": "Masterpiece",
    "rating": 10,
    "series": 1,
    "user": 5
}
```

## **PATCH Review**
Updates review.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}/review/{reviewId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
| reviewId | yes | 32 bit integer series ID | 1 |
| text | Yes | any string | "Masterpiece" |
| rating | Yes | integer 1 to 10 | 10 |
#### **Example Request**
```
PATCH https://{hostname}:{port}/api/genre/6/series/3/review/2
{
    "text": "This movie is good!",
    "rating": "9"
}
```
#### **Example Response**
```
Status 200
```

## **DELETE Review**
Deletes review.
#### **URL Template**
https://{hostname}:{port}/api/genre/{genreId}/series/{seriesId}/review/{reviewId}
#### **Parameters**
| Name | Required | Description | Example |
|-|-|-|-|
| genreId | yes | 32 bit integer genre ID | 1 |
| seriesId | yes | 32 bit integer series ID | 1 |
| reviewId | yes | 32 bit integer series ID | 1 |
#### **Example Request**
DELETE https://{hostname}:{port}/api/genre/6/series/3/review/2
#### **Example Response**
```
Status 200
```