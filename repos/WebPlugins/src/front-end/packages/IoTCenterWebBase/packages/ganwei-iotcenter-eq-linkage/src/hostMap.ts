
import routerUrl from '../../../configuration/moduleConfiguration.json'
const map = routerUrl.addressMapping;
export default function hostMap(host: string) {
    if (process.env.NODE_ENV === "production") return host;
    return map[host as keyof typeof map];
}
if (process.env.NODE_ENV === "production") {
    window.isProduction = true
} else {
    localStorage.hostMap = JSON.stringify(map)
}

hostMap._map = map;
