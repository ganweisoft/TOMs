package gwdatacenter.Interface;

/** 
 当有更新的时候，实现该接口，就不用重新启动服务
*/
public interface ICanReset
{
	boolean ResetWhenDBChanged(Object... o);
}