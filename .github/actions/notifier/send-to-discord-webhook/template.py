import re
from datetime import datetime

markdown_emojis = {
    "success": ":white_check_mark:",    # ✅
    "failure": ":x:",                   # ❌
}

status_colors = {
    "success": 65280,        # Green color
    "failure": 16711680,     # Red color
}

def camel_to_title(camel_str):
    s1 = re.sub('([a-z])([A-Z])', r'\1 \2', camel_str)
    s2 = re.sub('(_|-)', ' ', s1)
    return s2.title()


def create_field_item(key, value):
    return {
        "name": camel_to_title(key),
        "value": value,
        "inline": True
    }


def create_message(title, status="failure", author="GitHub", url=None, fields={}):

    emoji = markdown_emojis.get(status.lower(), markdown_emojis["failure"])
    color = status_colors.get(status.lower(), status_colors["failure"])

    now_iso = datetime.now().isoformat()

    # Optional
    url_string = f'[\u1CBC\u1CBC **Go to Workflow Logs** >>> :computer:]({url})' if url else "\u200B"

    # Fields
    field_list = [create_field_item(k, v) for k, v in fields.items()]

    # Embeds
    embeds = {
        "color": color,
        "title": f'{emoji} {title} {emoji}',
        "description": url_string,
        "timestamp": now_iso,
        "footer": {"text": author},
        "fields": field_list
    }

    message = {
        "embeds": [embeds]
    }

    return message
