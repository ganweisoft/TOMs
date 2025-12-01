package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;

public class DataItem {
	private int DeviceId;

	public final int getDeviceId() {
		return DeviceId;
	}

	public final void setDeviceId(int value) {
		DeviceId = value;
	}

	private HashMap<Integer, Object> Attribute = new HashMap<Integer, Object>();

	public final HashMap<Integer, Object> getAttribute() {
		return Attribute;
	}

	public final void setAttribute(HashMap<Integer, Object> value) {
		Attribute = value;
	}
}