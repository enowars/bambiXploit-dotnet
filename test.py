#!/usr/bin/env python3

import json
import requests
import sys
from typing import Optional

TARGET = sys.argv[1] # The target's ip address is passed as an command line argument

session = requests.Session()

def exploit(hint: Optional[str], flag_store: Optional[int]):
    print(f'Attacking {TARGET} (flag_store={flag_store}, hint={hint}')
    # TODO implement exploit


# Some CTFs publish information ('flag hints') which help you getting individual flags (e.g. the usernames of users that deposited flags).
# Bambi CTF / ENOWARS flag hints:
attack_info = json.loads(requests.get('http://enowarstest.stronk.pw:5001/scoreboard/attack.json').text)
service_info = attack_info['services']['Pomelo']
team_info = service_info[TARGET] # Get the information for the current target
for round in team_info:
    round_info = team_info[round]
    for flag_store in round_info:
        store_info = round_info[flag_store]
        for flag_info in store_info:
            exploit(flag_info, flag_store) # flag_info will always be a string, which you might have to parse with json.loads

# In CTFs that do not publish flag hints you are on your own.
'''
exploit(None, None)
'''

