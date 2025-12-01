package gwdatacenter.mqservice;

import gwdatacenter.*;
import java.util.*;

public class StateItem {
	private int DeviceId;

	public final int getDeviceId() {
		return DeviceId;
	}

	public final void setDeviceId(int value) {
		DeviceId = value;
	}

	private String State;

	public final String getState() {
		return State;
	}

	public final void setState(String value) {
		State = value;
	}
}