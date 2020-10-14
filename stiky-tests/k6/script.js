import http from 'k6/http'
import { check, fail  } from 'k6'
import { Rate } from 'k6/metrics'
var failureRate = new Rate('check_failure_rate')
// const apiUrl = 'http://host.docker.internal:5000'
const apiUrl = 'http://35.223.68.13'
const username = 'load_test';
export let options = {
    vus: 10,
    stages: [
        { duration: '2m', target: 50 }
    ],
    thresholds: {
        // We want the 95th percentile of all HTTP request durations to be less than 500ms
        'http_req_duration': ['p(95)<500'],
        // Requests with the staticAsset tag should finish even faster
        'http_req_duration{staticAsset:yes}': ['p(99)<250'],
        // Thresholds based on the custom metric we defined and use to track application failures
        'check_failure_rate': [
            // Global failure rate should be less than 1%
            'rate<0.01',
            // Abort the test early if it climbs over 5%
            { threshold: 'rate<=0.05', abortOnFail: true },
        ],
    },
    token: ''
};

export function setup() {
    var url = apiUrl + '/User/login?username=' + username
    let response = http.post(url)
    let body = JSON.parse(response.body)
    if (!check(body.token, {
        'token must be defined': t => t
    })) {
        fail('token generation failed')
    }
    return { token: body.token}
}

export default function(data) {
    var url = apiUrl + '/message?message=' + Math.random().toString(36).replace(/[^a-z]+/g, '')
    var payload = JSON.stringify({ name: 'test user', message: 'test message'})
    var parameters = {
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${data.token}`
        }
    }
    let response = http.post(url, payload, parameters)
    let checkRes = check(response, {
        'status is 200': (r) => r.status === 200
    })

    failureRate.add(!checkRes)
};