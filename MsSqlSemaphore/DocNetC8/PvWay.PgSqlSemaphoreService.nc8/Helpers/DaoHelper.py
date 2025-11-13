class DaoHelper:
    @staticmethod
    def truncate_then_escape(value, max_len):
        if not value:
            return value
        val = value if len(value) <= max_len else value[:max_len - 3] + "..."
        return DaoHelper.escape(val)

    @staticmethod
    def escape(value):
        return value.replace("'", "''")

    # returns a ready to concat sql string in
    # the form 'yyyy-MM-dd HH:mm:ss.sss'
    
    @staticmethod
    def get_timestamp(utc_now):
        return f"'{utc_now:%Y-%m-%d %H:%M:%S.%f}'"[:-3]