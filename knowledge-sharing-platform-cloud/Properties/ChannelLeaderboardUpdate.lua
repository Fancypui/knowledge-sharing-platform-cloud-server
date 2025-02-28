--key of sorted set
local zsetKey = KEYS[1]
-- channel id
local channelId = ARGV[1]
-- new member count
local memberCount = tonumber(ARGV[2])
--maximum member size of sorted set
local maxSize = 500

-- Check if channel exists
local currentScore = redis.call('ZSCORE', zsetKey, channelId)
--if channel exist, update the member count score if it is currentScore is bigger than the score in cache
if currentScore then
    if memberCount > tonumber(currentScore) then
        redis.call('ZADD', zsetKey, memberCount, channelId)
    end
else
    local zsetSize = redis.call('ZCARD', zsetKey)
    --add into cache immediately if size still < 500
    if zsetSize < maxSize then
        redis.call('ZADD', zsetKey, memberCount, channelId)
    else
        local lastEntry = redis.call('ZRANGE', zsetKey, 0, 0, 'WITHSCORES')
        local lowestChannel = lastEntry[1]
        local lowestScore = tonumber(lastEntry[2])
        --compare with the lowest score, replace it if new channel id's member count is bigger than the lowest
        if memberCount > lowestScore then
            redis.call('ZREM', zsetKey, lowestChannel)
            redis.call('ZADD', zsetKey, memberCount, channelId)
        end
    end
end
