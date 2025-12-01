from typing import Any


class ICanReset:
    def ResetWhenDBChanged(self, *args: Any) -> bool:
        pass
