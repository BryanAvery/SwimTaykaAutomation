/*
 * This software is developed for SwimTayka, a charity dedicated to teaching 
 * swimming and water safety to underprivileged children around the globe.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * If you reuse any part of this code, please consider making a donation to 
 * SwimTayka to support their cause. For donation information, please visit:
 * https://swimtayka.org/donate
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwimTaykaStreak
{
    /// <summary>
    /// Defines an interface for interacting with the Streak API to manage boxes and fields within them.
    /// </summary>
    public interface IStreakClient
    {
        /// <summary>
        /// Asynchronously retrieves a list of box keys associated with a specific pipeline.
        /// </summary>
        /// <param name="boxKey">The key of the pipeline from which the box keys are retrieved.</param>
        /// <returns>A task that represents the asynchronous operation and contains a list of box keys if successful; otherwise, null.</returns>
        Task<List<string>> GetBoxKeysAsync(string streakKeyApi, string boxKey);

        /// <summary>
        /// Asynchronously retrieves the URL of a specific box based on its key.
        /// </summary>
        /// <param name="boxKey">The key of the box for which the URL is retrieved.</param>
        /// <returns>A task that represents the asynchronous operation and contains the URL of the box if successful; otherwise, an exception is thrown.</returns>
        Task<string> GetBoxUrlAsync(string streakKeyApi, string boxKey, string fieldId);

        /// <summary>
        /// Posts a new value to a specific field in a box and asynchronously returns the response from the server.
        /// </summary>
        /// <param name="boxKey">The key of the box containing the field to be updated.</param>
        /// <param name="value">The new value to be posted to the field.</param>
        /// <returns>A task that represents the asynchronous operation and contains the server response if successful; otherwise, an error message or exception details.</returns>
        Task<string> PostStreakField(string streakKeyApi, string boxKey, string value, string fieldToUpdateId);
    }
}
