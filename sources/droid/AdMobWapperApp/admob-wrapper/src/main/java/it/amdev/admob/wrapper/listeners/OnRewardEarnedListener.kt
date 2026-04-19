package it.amdev.admob.wrapper.listeners

interface OnRewardEarnedListener {
    fun onRewardEarned(type: String, amount: Int)
}