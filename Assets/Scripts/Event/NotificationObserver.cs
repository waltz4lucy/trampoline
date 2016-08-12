using UnityEngine;
using System.Collections;

public class NotificationObserver {
        public object target { get; private set; }
        public string method { get; private set; }

        public NotificationObserver(object target, string method) {
            this.target = target;
            this.method = method;
        }

        public override bool Equals(object obj) {
            return this.Equals(obj as NotificationObserver);
        }

        public bool Equals(NotificationObserver o) {
            // o is null, return false
            if (object.ReferenceEquals(o, null)) {
                return false;
            }

            if (object.ReferenceEquals(this, o)) {
                return true;
            }

            if (this.GetType() != o.GetType()) {
                return false;
            }

            return (this.target == o.target) && (this.method.Equals(o.method));
        }

        public static bool operator ==(NotificationObserver lhs, NotificationObserver rhs) {
            // Check for null on left side. 
            if (object.ReferenceEquals(lhs, null))
            {
                if (object.ReferenceEquals(rhs, null))
                {
                    // null == null = true. 
                    return true;
                }

                // Only the left side is null. 
                return false;
            }
            // Equals handles case of null on right side. 
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NotificationObserver lhs, NotificationObserver rhs) {
            return !(lhs == rhs);
        }

        public override int GetHashCode() {
            return this.target.GetHashCode() * 17 + this.method.GetHashCode();
        }
    }
