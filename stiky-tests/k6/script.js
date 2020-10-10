import http from "k6/http";
import { check, sleep  } from "k6";
import { Rate } from "k6/metrics";
var failureRate = new Rate("check_failure_rate");
export let options = {
    vus: 10,
    stages: [
        { duration: "1m", target: 10 },
        { duration: "2m", target: 10 },
        { duration: "30s", target: 35 },
        { duration: "30s", target: 0 },
    ],
    thresholds: {
        // We want the 95th percentile of all HTTP request durations to be less than 500ms
        "http_req_duration": ["p(95)<500"],
        // Requests with the staticAsset tag should finish even faster
        "http_req_duration{staticAsset:yes}": ["p(99)<250"],
        // Thresholds based on the custom metric we defined and use to track application failures
        "check_failure_rate": [
            // Global failure rate should be less than 1%
            "rate<0.01",
            // Abort the test early if it climbs over 5%
            { threshold: "rate<=0.05", abortOnFail: true },
        ],
    },
};
export default function() {
    var url = 'http://172.21.0.1:5000/message';
    var payload = JSON.stringify({ name: "test user", message: 'test message'});
    var params = {
        headers: {
        'Content-Type': 'application/json',
        },
    };
    let response = http.post(url, payload, params);
    let checkRes = check(response, {
        "status is 200": (r) => r.status === 200
    });

    failureRate.add(!checkRes);
};