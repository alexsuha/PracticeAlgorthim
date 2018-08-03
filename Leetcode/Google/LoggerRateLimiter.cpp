/*
 * Problem:
 *
 * Design a logger system that receive stream of messages along with its timestamps, each message should be printed if and only if it is not printed in the last 10 seconds.
 *
 * Given a message and timestamp (in seconds granularity), return true if the message should be printed in the given timestamp, otherwise return false.
 *
 * It is possible that several messages arrive roughly at the same time.
 *
 *  Example:
 *
 *  Logger logger = new logger();
 *
 *  // logging string "foo" at timestamp 1
 *  logger.shouldPrintMessage(1, "foo"); return true;
 *
 *  // logging string "bar" at timestamp 2
 *  logger.shouldPrintMessage(2, "bar"); return true;
 *
 *  // logging string "bar" at timestamp 3
 *  logger.shouldPrintMessage(3, "bar"); return false;
 *  
 *  // logging string "foo" at timestamp 10
 *  logger.shouldPrintMessage(10, "foo"); return false;
 *  
 *  // logging string "foo" at timestamp 11
 *  logger.shouldPrintMessage(11, "foo"); return true;
 *
 */

#include <iostream>
#include <unordered_map>
#include <string>

using namespace std;

class Logger {
public:
      /** Initialize your data structure here. */
      Logger() {
      }

      /** Returns true if the message should be printed in the given timestamp, otherwise returns false.
       * If this method returns false, the message will not be printed.
       * The timestamp is in seconds granularity. */

      bool shouldPrintMessage(int timestamp, string message) {
	   if(logManager.find(message) == logManager.end()
			   || timestamp - logManager[message] > 9)
	   {
		   logManager[message] = timestamp;
		   return true;
	   }
	   return false;
      }

private:
      unordered_map<string, int> logManager;
};

int main()
{
	Logger obj;
	bool res = false;
	res = obj.shouldPrintMessage(1, "foo");
	cout << res << endl;
	res = obj.shouldPrintMessage(2, "foo");
	cout << res << endl;
	res = obj.shouldPrintMessage(11, "foo");
	cout << res << endl;

        return 0;
}
