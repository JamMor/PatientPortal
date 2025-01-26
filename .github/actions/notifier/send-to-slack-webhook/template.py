import re
from datetime import datetime
from date_formatter import get_slack_formatted_datetime_from_iso

markdown_emojis = {
    "success": ":white_check_mark:",    # ✅
    "failure": ":x:",                   # ❌
}

def camel_to_title(camel_str):
    s1 = re.sub('([a-z])([A-Z])', r'\1 \2', camel_str)
    s2 = re.sub('(_|-)', ' ', s1)
    return s2.title()


def create_markdown_field(key, value):
    return {
        "type": "mrkdwn",
        "text": f'*{camel_to_title(key)}:*\n{value}'
    }


def create_message(title, status="failure", url=None, fields={}):

    emoji = markdown_emojis.get(status.lower(), markdown_emojis["failure"])

    # Get the formatted datetime
    datetime_iso = datetime.now().isoformat()
    num_datetime = get_slack_formatted_datetime_from_iso(
        iso_date_str=datetime_iso, date_format="date_num", time_format="time_secs")
    pretty_datetime = get_slack_formatted_datetime_from_iso(
        iso_date_str=datetime_iso, date_format="date_long_pretty")

    divider_block = {
        "type": "divider"
    }

    # Title
    title_block = {
        "type": "header",
        "text": {
            "type": "plain_text",
            "text": f'{emoji} {title} {emoji}',
            "emoji": True
        }
    }

    # Subtitle - Event Time
    subtitle_block = {
        "type": "section",
        "text": {
            "type": "mrkdwn",
            "text": f'*Event Time:* {num_datetime}'
        }
    }

    # Footer - Timestamp
    footer_block = {
        "type": "context",
        "elements": [
            {
                "type": "mrkdwn",
                "text": pretty_datetime
            }
        ]
    }

    # Optional blocks
    url_block = {
        "type": "section",
        "text": {
            "type": "mrkdwn",
            "text": "View in AWS Browser Console."
        },
        "accessory": {
            "type": "button",
            "text": {
                "type": "plain_text",
                "text": ":computer: Go to Workflow Logs",
                "emoji": True
            },
            "url": url,
        }
    } if url else None

    # Fields
    field_list = [create_markdown_field(k, v) for k, v in fields.items()]

    field_block = {
        "type": "section",
        "fields": field_list
    } if field_list else None

    # Message Structure
    blocks = [
        title_block,
        subtitle_block,
        divider_block,
        field_block,
        divider_block,
        url_block,
        footer_block
    ]
    # Remove Absent Blocks
    if not url_block:
        del blocks[5]
    if not field_block:
        # Remove the divider as well
        del blocks[3:4]

    message = {
        "text": title,
        "blocks": blocks
    }

    return message
