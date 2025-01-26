import os
import argparse
import json
from template import create_message

def main():
    parser = argparse.ArgumentParser(description='Create a message.')
    parser.add_argument('--title', required=True, help='The title of the message.')
    parser.add_argument('--status', default='failure', help='The job status.')
    parser.add_argument('--author', default='GitHub', help='The author of the message.')
    parser.add_argument('--url', help='The URL of the workflow run.')
    parser.add_argument('--fields', type=json.loads, default={}, help='A JSON string of fields of form \{name1\:val1,name2\:val2\}.')

    args = parser.parse_args()

    message = create_message(
        title = args.title,
        status = args.status,
        author = args.author,
        url = args.url,
        fields = args.fields
    )

    with open('discord-message.json', 'w') as output_file:
        json.dump(message, output_file)

if __name__ == "__main__":
    main()