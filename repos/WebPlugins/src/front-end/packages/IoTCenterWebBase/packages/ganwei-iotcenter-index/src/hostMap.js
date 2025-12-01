
import routerUrl from '../../../configuration/moduleConfiguration.json'
const map = routerUrl.addressMapping;
export default function hostMap (host) {
    if (process.env.NODE_ENV === "production") return host;
    return map[host];
}
if (process.env.NODE_ENV === "production") {
    window.top.isProduction = true
} else {
    localStorage.hostMap = JSON.stringify(map)
}
hostMap._map = map;
