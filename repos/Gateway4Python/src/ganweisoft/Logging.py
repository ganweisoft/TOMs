import os


class Logging:

    @classmethod
    def write_log_file(cls, input_str):
        print(input_str)
        try:
            root_path = os.path.join(os.getcwd(), "log")
            os.makedirs(root_path, exist_ok=True)
            file_name = "XLog.txt"
            file_path = os.path.join(root_path, file_name)

            if not os.path.exists(file_path):
                with open(file_path, 'w') as f:
                    pass
            else:
                file_info = os.stat(file_path)
                if file_info.st_size > 1024 * 1024 * 5:
                    os.remove(file_path)
                    with open(file_path, 'w') as f:
                        pass

            with open(file_path, 'ab') as f:
                sss = "服务端"
                str_now = f"*******************************************{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}({sss})*******************************************\n"
                f.write(str_now.encode('utf-8'))

                f.write(input_str.encode('utf-8'))
                f.write(b"\n")

                fg = "**********************************************************************************************\n"
                f.write(fg.encode('utf-8'))

                f.write(b"\n")
        except Exception as e:
            pass