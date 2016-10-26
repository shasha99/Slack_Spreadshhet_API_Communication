import requests;
import sys;


class SlackError(Exception):
    pass

class SlackClient(object):
    BASE_URL = 'https://slack.com/api'

    def __init__(self, token):
        self.token = token
        self.blocked_until = None
        self.channel_name_id_map = {}
		
    def chat_post_message(self, channel, text, **params):
        method = 'chat.postMessage'
        params.update({
            'channel': channel,
            'text': text,
        })
        return self._make_request(method, params)
		
    def _make_request(self, method, params):
        if self.blocked_until is not None and \
                datetime.datetime.utcnow() < self.blocked_until:
            raise SlackError("Too many requests - wait until {0}" \
                    .format(self.blocked_until))

        url = "%s/%s" % (SlackClient.BASE_URL, method)
        params['token'] = self.token
        response = requests.post(url, data=params, verify=False)

        if response.status_code == 429:
            # Too many requests
            retry_after = int(response.headers.get('retry-after', '1'))
            self.blocked_until = datetime.datetime.utcnow() + \
                    datetime.timedelta(seconds=retry_after)
            raise SlackError("Too many requests - retry after {0} second(s)" \
                    .format(retry_after))

        result = response.json()
        if not result['ok']:
            raise SlackError(result['error'])
        return result
		
client = SlackClient(sys.argv[0])
client.chat_post_message('#roar', "A Roar from th Jungle again !!!", username='shashank awasthi')