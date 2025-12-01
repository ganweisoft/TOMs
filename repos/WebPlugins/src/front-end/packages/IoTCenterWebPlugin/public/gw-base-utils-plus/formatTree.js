export default function formateList (list,cache,level,parentKey=0) {
    let arr = []
    for (const item of list) {
        let dataItem = {}
        dataItem.count = item.count
        dataItem.title = item.groupName
        dataItem.key = item.groupId
        dataItem.isGroup = true
        dataItem.children = []
        dataItem.status = 1
        dataItem.level = level||1
        dataItem.expand = !level;
        dataItem.equips=[]
        dataItem.groups = []
        dataItem.parentKey=parentKey

        if (item.equipLists.length > 0) {
            if(cache){
                window[`${dataItem.key}-${dataItem.title}`] = deepClone(item.equipLists)
            }else{
                dataItem.children = [
                    ...dataItem.children,
                    ...item.equipLists.map((equip) => {
                        return {
                            count: 0,
                            title: equip.equipName,
                            equipState: equip.equipState,
                            key: equip.equipNo,
                            staNo: equip.staNo,
                            status: equip.equipState,
                            procAdvice: equip.procAdvice || ''
                        }
                    })
                ]
            }

        }
        if (item.children.length > 0) {
            dataItem.groups = [
                // ...dataItem.groups,
                ...formateList(item.children,cache,level?level+1:2,dataItem.key)
            ]
        }

        arr.push(dataItem)
    }
    return arr
}

function deepClone(source) {
    let arr = [];
    if (source) {
      for (let i = 0, length = source.length; i < length; i++) {
        arr.push(
          {
            count: source[i].count,
            groupId: source[i].groupId,
            isGroup: false,
            key: (Object.prototype.toString.call(source[i].equipNo).includes('String')) && source[i].equipNo.includes('-') ? source[i].equipNo : `${source[i].equipNo}-${source[i].equipNo}`,
            procAdvice: source[i].procAdvice,
            relatedView: source[i].relatedView,
            staNo: source[i].staNo,
            status: source[i].status,
            title: source[i].equipName,
            parentKey: source[i].parentKey,
            parentTitle: source[i].parentTitle
          }
        )
      }
    }
  
    return arr;
  }