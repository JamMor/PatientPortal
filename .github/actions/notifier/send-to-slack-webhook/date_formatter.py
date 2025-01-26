from datetime import datetime


slack_date_formats = {
    "date_num": "{date_num}",                   # 2014-02-18. Includes leading zeros before the month and date.
    "date": "{date}",                           # February 18th, 2014. Year will be omitted if the date is within six months.
    "date_short": "{date_short}",               # Feb 18, 2014. Year will be omitted if the date is within six months.
    "date_long": "{date_long}",                 # Tuesday, February 18th, 2014. Year will be omitted if the date is within six months.
    "date_pretty": "{date_pretty}",             # same as {date} but uses "yesterday", "today", or "tomorrow" where appropriate.
    "date_short_pretty": "{date_short_pretty}", # same as {date_short} but uses "yesterday", "today", or "tomorrow" where appropriate.
    "date_long_pretty": "{date_long_pretty}",   # same as {date_long} but uses "yesterday", "today", or "tomorrow" where appropriate.
}
slack_time_formats = {
    "time": "{time}",           # 6:39 PM in 12-hour format.    18:39 in 24-hour format.
    "time_secs": "{time_secs}", # 6:39:45 PM in 12-hour format. 18:39:42 in 24-hour format.
}


def get_slack_formatted_datetime_string(unix_date_str, iso_date_str=None, date_format="date_num", time_format="time_secs"):
    if date_format not in slack_date_formats:
        date_format = "date_num"
    if time_format not in slack_time_formats:
        time_format = "time_secs"
    if iso_date_str is None:
        date_obj = datetime.fromtimestamp(float(unix_date_str))
        iso_date_str = date_obj.isoformat(timespec='milliseconds')

    # Slack date format:
    # <!date^timestamp^token_string^optional_link|fallback_text>
    # Token String: "{date_format} {time_format}"
    return f'<!date^{unix_date_str}^{slack_date_formats[date_format]} {slack_time_formats[time_format]}|{iso_date_str}>'


def get_slack_formatted_datetime_from_iso(iso_date_str, date_format="date_num", time_format="time_secs"):
    date_obj = datetime.fromisoformat(iso_date_str)
    unix_time = int(round(date_obj.timestamp()))
    unix_time_str = str(unix_time)
    return get_slack_formatted_datetime_string(unix_date_str=unix_time_str, iso_date_str=iso_date_str, date_format=date_format, time_format=time_format)
