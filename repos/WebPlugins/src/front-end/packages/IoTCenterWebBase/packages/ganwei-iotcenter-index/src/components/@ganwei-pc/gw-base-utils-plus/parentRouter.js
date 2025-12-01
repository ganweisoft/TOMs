const parentRouter = {
    push: (route)=>{
        window.parent.postMessage({parentRouter: route},'*')
    }
}
export default parentRouter
