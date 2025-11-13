from typing import Callable, Optional
from abc import ABC, abstractproperty

class ISemaphoreServiceConfig(ABC):
    @property
    @abstractproperty
    def SchemaName(self) -> str:
        pass

    @property
    @abstractproperty
    def TableName(self) -> str:
        pass

    @property
    @abstractproperty
    def GetCsAsync(self) -> Callable[[], 'Task[str]']:
        pass

    @property
    @abstractproperty
    def LogException(self) -> Callable[[Exception], None]:
        pass

    @property
    @abstractproperty
    def LogInfo(self) -> Optional[Callable[[str], None]]:
        pass