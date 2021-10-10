# Slickdeals notifier in .NET core

Console application that scrapes www.slickdeals.net and notifies when there are any deals with more than 100 votes.

Uses SQLite to make sure it doesn't notify duplicate deals.

## Email notification setup

If you want to run the notifier and enable email notifications when a deal meets your notification criteria, you must create a [SendGrid](https://www.twilio.com/sendgrid/email-api) account and follow the instructions to generate an API Key and Single Sender Verification.

### Setting up your Sendgrid API Key

To set up your SendGrid API Key, follow the [Email Web API](https://app.sendgrid.com/guide/integrate) steps in the SendGrid app, then create an environment variable called `SLICKDEALS_SENDGRID_APIKEY` and set its value to the API Key generated there.

### Setting the "mail from" and "email to" addresses

Go through the [Single Sender Verification](https://app.sendgrid.com/settings/sender_auth/senders/new) steps in the SendGrid app, then update appsettings.config with the email address you used for the verification.

```json
{
  "EmailFrom": "single-sender-email@mail.com",
  "EmailTo": "any-email-you-want@mail.com"
}
```

## Contributions

PRs are welcome! If you see any area of improvement, feel free to create a PR to address it.
