export default{
    methods:{
        deepClone(source) {
            let arr = [];
            if (source) {
              for (let i = 0, length = source.length; i < length; i++) {
                if (typeof source[i].key == 'string') {
                  this.originEquipMap[source[i].key.split('-')[1]] = source[i].key
                } else {
                  this.originEquipMap[source[i].key] = `${source[i].groupId}-${source[i].key}`
                }
                arr.push(
                  {
                    count: source[i].count,
                    groupId: source[i].groupId,
                    isGroup: source[i].isGroup,
                    key: (Object.prototype.toString.call(source[i].key).includes('String')) && source[i].key.includes('-') ? source[i].key : `${source[i].groupId}-${source[i].key}`,
                    procAdvice: source[i].procAdvice,
                    relatedView: source[i].relatedView,
                    staNo: source[i].staNo,
                    status: source[i].status,
                    title: source[i].title,
                    parentKey: source[i].parentKey,
                    parentTitle: source[i].parentTitle
                  }
                )
              }
            }
          
            return arr;
          }
    }
}