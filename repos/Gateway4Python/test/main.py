import os
import sys

from src.ganweisoft.DataCenter import DataCenter

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

if __name__ == '__main__':
    DataCenter.start()
