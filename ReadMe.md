
# SwimTaykaStreak

This project is developed for SwimTayka, a charity dedicated to teaching swimming and water safety to underprivileged children around the globe. The codebase integrates functionalities for updating fundraising amounts collected via JustGiving into a Streak CRM.

## Prerequisites

Before you begin, ensure you have met the following requirements:
- .NET Core 3.1 SDK or later
- Azure Functions Core Tools
- An active Azure subscription
- Access to Streak CRM API
- Access to JustGiving fundraising pages

## Installation

To install SwimTaykaStreak, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SwimTaykaStreak.git
   ```
2. Navigate to the project directory:
   ```bash
   cd SwimTaykaStreak
   ```
3. Install the necessary packages:
   ```bash
   dotnet restore
   ```

## Configuration

To configure the project, you need to set up the necessary environment variables:
- `StreakApiKey`: The API key for accessing Streak CRM.
- `JustGivingApiKey`: The API key for accessing JustGiving.

These keys can be set in local.settings.json for local development or configured in the Azure portal for production environments.

## Usage

To run SwimTaykaStreak locally, execute:
```bash
dotnet run
```

To deploy to Azure Functions, use:
```bash
func azure functionapp publish <Your Function App Name>
```

The function `UpdateJustGivingRaisedAmountInStreak` can be triggered via HTTP requests. It expects a `boxKey` query parameter that corresponds to specific fundraising boxes in Streak.


# UpdateJustGivingRaisedAmountInStreak Endpoint

## Overview
This Azure Function, named `UpdateJustGivingRaisedAmountInStreak`, is designed to update fundraising amounts in Streak CRM based on data scraped from JustGiving pages. It is triggered via an HTTP GET request.

## HTTP Request
- **Method**: GET
- **Route**: Not explicitly set; defaults to the function name

## Query Parameters
- **`boxKey`** (required): A unique identifier for the box in Streak CRM. Necessary to fetch and update the corresponding box.
- **`fieldId`** (required): The identifier for the field in the Streak box where the JustGiving page URL is stored. This field is used to obtain the fundraising URL.
- **`fieldToUpdateId`** (required): The identifier for the field in the Streak box where the updated fundraising amount should be posted.

## Responses
- **Success**: Returns an `OkObjectResult` containing a list of URLs for the JustGiving pages whose associated fundraising amounts were successfully updated. Each URL in the list indicates a successfully updated box.
- **Failure**:
  - Returns a `BadRequestObjectResult` if any of the required query parameters (`boxKey`, `fieldId`, `fieldToUpdateId`) are missing.
  - Returns a `NotFoundObjectResult` if no box keys are found for the provided `boxKey`.

## Logging
The function logs the start and completion of the operation, along with any significant events or errors during the process.

## Implementation Notes
- The function retrieves all related box keys from Streak CRM using the `boxKey` parameter.
- For each box key, it fetches the URL of the JustGiving page from the specified field (`fieldId`), scrapes the current fundraising amount, and updates the Streak field specified by `fieldToUpdateId`.
- It uses an asynchronous approach to manage multiple updates concurrently, improving performance and scalability.

## Usage Example
To invoke this function, you would send an HTTP GET request with the required parameters. For instance:

\`\`\`
GET /api/UpdateJustGivingRaisedAmountInStreak?boxKey=exampleBoxKey&fieldId=exampleFieldId&fieldToUpdateId=exampleFieldToUpdateId
\`\`\`

This function is crucial for automating the update of fundraising information in Streak CRM, reducing manual data entry and ensuring up-to-date records are maintained.

## Contributing

Contributions to the SwimTaykaStreak project are welcome. If you have a suggestion that would improve this project, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This software is distributed under the GNU General Public License (GPL). For more details, see the LICENSE file in the repository.

## Support the Cause

If you reuse any part of this code, please consider making a donation to SwimTayka to support their cause. For donation information, please visit: [Donate to SwimTayka](https://swimtayka.org/donate).

